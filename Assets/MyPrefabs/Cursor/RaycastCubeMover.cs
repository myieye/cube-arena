using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.Interaction.Abstract;
using CubeArena.Assets.MyScripts.Interaction.Listeners;
using CubeArena.Assets.MyScripts.Interaction.State;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyPrefabs.Cursor {
	public class RaycastCubeMover : AbstractCubeMover, CubeDeselectedListener, UIModeChangedListener {

		[SerializeField]
		[SyncVar]
		private float dragDistanceThreshold = 1f;
		[SerializeField]
		protected float cubeOffset = 0.5f;
		private float currOffset;
		protected float cubeRadius = 1.2f;
		protected Rigidbody cubeRb;
		private NavMeshObstacle cubeNavObstacle;
		private OverlapManager overlapManager;
		protected EnabledComponent<CursorController> cursor;

		protected GameObject dragCubeTarget;
		protected Vector3 dragStartPosition;

		protected override void Start () {
			base.Start ();
			stateManager.AddOnCubeDeselectedListener (this);
			cursor = new EnabledComponent<CursorController> (gameObject);
			ResetOffset ();
			UIModeManager.RegisterUIModeChangedListener (this);
		}

		private void OnDestroy () {
			UIModeManager.UnregisterUIModeChangedListener (this);
		}

		protected override void Update () {
			base.Update ();
			if (StuckInDisallow ()) {
				currOffset += 0.1f;
			}
			if (CrossPlatformInputManager.GetButtonDown (Buttons.Select) && stateManager.IsHovering ()) {
				dragCubeTarget = stateManager.HoveredCube.Cube;
				dragStartPosition = transform.position;
			} else if (CrossPlatformInputManager.GetButtonUp (Buttons.Select)) {
				dragCubeTarget = null;
			}
		}

		protected override bool IsStartingMove (out GameObject cube) {
			cube = dragCubeTarget;
			//cube = stateManager.IsHovering () ? stateManager.HoveredCube.Cube : null;
			return //stateManager.IsHovering () &&
			dragCubeTarget &&
				//DragWasStartedOnCube (stateManager.HoveredCube.Cube) &&
				(HasMoved () /* || stateManager.IsManipulating*/ );
		}

		protected override void StartMove (GameObject cube) {
			var box = cube.GetComponent<BoxCollider> ();
			cubeRadius = (box.size.x * cube.transform.lossyScale.x) / 2;
			Debug.Log ("cubeRadius:" + cubeRadius);
			
			SelectCubeOnNetwork (cube);
		}

		protected override bool IsEndingMove () {
			return CrossPlatformInputManager.GetButtonUp (Buttons.Select);
		}

		protected override void EndMove () {
			EndMoveLocal ();
			CmdDeselectMovingCube (stateManager.SelectedCube.Cube);
		}

		private void EndMoveLocal () {
			if (cubeRb) {
				DeselectCube (cubeRb.gameObject);
				ResetOffset ();
			}
		}

		protected override bool IsDisallowed () {
			return overlapManager != null ? overlapManager.HasOverlap () : false;
		}

		protected override void Move () {
			if (cubeRb != null) {
				cubeRb.velocity = cursor.Get.GetVelocityFromToOffset (
					stateManager.SelectedCube.Cube.transform.position, currOffset);
			}
		}

		protected void SelectCubeOnNetwork (GameObject cube) {
			SelectCube (cube);
			CmdSelectCubeOnNetwork (cube);
		}

		[Command]
		protected void CmdSelectCubeOnNetwork (GameObject cube) {
			RpcSelectCubeOnOtherClients (cube);
		}

		[ClientRpc]
		void RpcSelectCubeOnOtherClients (GameObject cube) {
			if (!hasAuthority && cube) {
				lock (this) {
					SelectCube (cube);
				}
			}
		}

		private void SelectCube (GameObject cube) {
			var rb = cube.GetComponent<Rigidbody> ();
			if (cubeRb && !(rb == cubeRb)) {
				DeselectCube (cubeRb.gameObject);
			}
			cubeRb = rb;
			cube.layer = Layers.IgnoreRaycast;
			cube.GetComponent<Collider> ().isTrigger = true;
			cubeRb.useGravity = false;
			cubeRb.constraints = RigidbodyConstraints.FreezeRotation;
			cubeNavObstacle = cube.GetComponent<NavMeshObstacle> ();
			cubeNavObstacle.enabled = false;

			if (hasAuthority) {
				overlapManager = cube.GetComponent<OverlapManager> ();
				cursor.Get.Refresh ();
			}
		}

		[Command]
		protected void CmdDeselectMovingCube (GameObject cube) {
			RpcDeselectMovingCube (cube);
		}

		[ClientRpc]
		void RpcDeselectMovingCube (GameObject cube) {
			if (!hasAuthority && cube) {
				lock (this) {
					DeselectCube (cube);
				}
			}
		}

		private void DeselectCube (GameObject cube) {
			cube.layer = Layers.Cubes;
			cube.GetComponent<Collider> ().isTrigger = false;

			var rb = cube.GetComponent<Rigidbody> ();
			rb.constraints = RigidbodyConstraints.None;
			rb.useGravity = true;
			rb.velocity = Vector3.zero;
			if (rb == cubeRb) {
				cubeRb = null;
			}

			var no = cube.GetComponent<NavMeshObstacle> ();
			no.enabled = true;
			if (no == cubeNavObstacle) {
				cubeNavObstacle = null;
			}
		}

		protected virtual void ResetOffset () {
			currOffset = cubeOffset + cubeRadius;
		}

		bool StuckInDisallow () {
			return stateManager.InState (InteractionState.Disallowed) &&
				cubeRb.velocity.magnitude < 4 &&
				overlapManager.HasOverlapWith (cursor.Get.GetAlignedWith ());
		}

		bool HasMoved () {
			return (transform.position - dragStartPosition).magnitude > dragDistanceThreshold;
		}

		bool DragWasStartedOnCube (GameObject cube) {
			return dragCubeTarget != null && dragCubeTarget == cube;
		}

		public virtual void OnCubeDeselected (GameObject cube) {
			if (cubeRb) {
				EndMoveLocal ();
				CmdDeselectMovingCube (cube);
			}
		}

		public virtual void OnUIModeChanged (UIMode mode) {
			ResetOffset ();
		}
	}
}