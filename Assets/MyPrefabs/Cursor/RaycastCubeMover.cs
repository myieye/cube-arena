using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.Interaction.Abstract;
using CubeArena.Assets.MyScripts.Interaction.Listeners;
using CubeArena.Assets.MyScripts.Interaction.State;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyPrefabs.Cursor {
	public class RaycastCubeMover : AbstractCubeMover, OnCubeDeselectedListener {

		public float cubeOffset = 0.5f;
		private float currOffset;
		protected Rigidbody cubeRb;
		private NavMeshObstacle cubeNavObstacle;
		private OverlapManager overlapManager;
		protected CursorController cursorCtrl;
		protected GameObject dragCubeTarget;

		protected override void Start () {
			base.Start ();
			stateManager.AddOnCubeDeselectedListener (this);
			cursorCtrl = GetComponentInParent<CursorController> ();
			ResetOffset ();
		}

		protected override void Update () {
			base.Update ();
			if (StuckInDisallow ()) {
				currOffset += 0.1f;
			}
			if (CrossPlatformInputManager.GetButtonDown (Buttons.Select) && stateManager.IsHovering ()) {
				dragCubeTarget = stateManager.HoveredCube.Cube;
			} else if (CrossPlatformInputManager.GetButtonUp (Buttons.Select)) {
				dragCubeTarget = null;
			}
		}

		protected override bool IsStartingMove (out GameObject cube) {
			cube = stateManager.IsHovering () ? stateManager.HoveredCube.Cube : null;
			return
			//CrossPlatformInputManager.GetButton(Buttons.Select) &&
			//stateManager.IsHovering() && (!stateManager.HasSelection() ||
			//	stateManager.IsSelected(stateManager.HoveredCube.Cube)) &&
			stateManager.IsHovering () &&
				IsMoving () && DragWasStartedOnCube (stateManager.HoveredCube.Cube);
		}

		protected override void StartMove (GameObject cube) {
			SelectCubeOnNetwork (cube);
		}

		protected override bool IsEndingMove () {
			return CrossPlatformInputManager.GetButtonUp (Buttons.Select);
		}

		protected override void EndMove () {
			CmdDeselectMovingCube (stateManager.SelectedCube.Cube);
		}

		protected override bool IsDisallowed () {
			return overlapManager != null ? overlapManager.HasOverlap () : false;
		}

		protected override void Move () {
			if (cubeRb != null) {
				cubeRb.velocity = cursorCtrl.GetVelocityFromToOffset (
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
			if (!hasAuthority) {
				SelectCube (cube);
			}
		}

		private void SelectCube (GameObject cube) {
			cube.layer = Layers.IgnoreRaycast;
			cube.GetComponent<Collider> ().isTrigger = true;
			cubeRb = cube.GetComponent<Rigidbody> ();
			cubeRb.useGravity = false;
			cubeRb.constraints = RigidbodyConstraints.FreezeRotation;
			cubeNavObstacle = cube.GetComponent<NavMeshObstacle> ();
			cubeNavObstacle.enabled = false;

			if (hasAuthority) {
				overlapManager = cube.GetComponent<OverlapManager> ();
				GetComponent<CursorController> ().Refresh ();
			}
		}

		[Command]
		protected void CmdDeselectMovingCube (GameObject cube) {
			RpcDeselectMovingCube (cube);
		}

		[ClientRpc]
		void RpcDeselectMovingCube (GameObject cube) {
			lock (this) {
				if (cubeRb != null) {
					cube.layer = Layers.Default;
					cube.GetComponent<Collider> ().isTrigger = false;
					cubeRb.constraints = RigidbodyConstraints.None;
					cubeRb.useGravity = true;
					cubeRb.velocity = Vector3.zero;
					cubeRb = null;
					ResetOffset ();
					cubeNavObstacle.enabled = true;
					cubeNavObstacle = null;

					if (hasAuthority) {
						overlapManager = null;
					}
				}
			}
		}

		void ResetOffset () {
			currOffset = cubeOffset + 1;
		}

		bool StuckInDisallow () {
			return stateManager.InState (InteractionState.Disallowed) &&
				cubeRb.velocity.magnitude < 2 &&
				overlapManager.HasOverlapWith (cursorCtrl.GetAlignedWith ());
		}

		bool IsMoving () {
			return cursorCtrl.IsMoving ();
		}

		bool DragWasStartedOnCube (GameObject cube) {
			return dragCubeTarget != null && dragCubeTarget == cube;
		}

		public virtual void OnCubeDeselected (GameObject cube) {
			if (cubeRb != null) {
				CmdDeselectMovingCube (cube);
			}
		}
	}
}