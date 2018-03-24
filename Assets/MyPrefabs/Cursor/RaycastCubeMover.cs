using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Constants;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.Utils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyPrefabs.Cursor
{
	public class RaycastCubeMover : AbstractCubeMover {

		public float cubeOffset = 0.5f;
		private float currOffset;
		private Rigidbody cubeRb;
		private NavMeshObstacle cubeNavObstacle;
		private OverlapManager overlapManager;
		private CursorController cursorCtrl;
		private GameObject dragCubeTarget;

		protected override void Start () {
			base.Start();
			cursorCtrl = GetComponentInParent<CursorController>();
			ResetOffset();
		}

		protected override void Update () {
			base.Update();
			if (StuckInDisallow()) {
				currOffset += 0.1f;
			}

			if (CrossPlatformInputManager.GetButtonDown(Buttons.Select) && stateManager.IsHovering()) {
				dragCubeTarget = stateManager.HoveredCube.Cube;
			} else if (CrossPlatformInputManager.GetButtonUp(Buttons.Select)) {
				dragCubeTarget = null;
			}
		}

		protected override bool IsStartingMove() {
			var starting =
				//CrossPlatformInputManager.GetButton(Buttons.Select) &&
				//stateManager.IsHovering() && (!stateManager.HasSelection() ||
				//	stateManager.IsSelected(stateManager.HoveredCube.Cube)) &&
				stateManager.IsHovering() &&
				IsMoving() && DragWasStartedOnCube(stateManager.HoveredCube.Cube);
			if (starting) {
				CmdSelectMovingCube(stateManager.HoveredCube.Cube);
			}
			return starting;
		}

		protected override bool IsEndingMove() {
			if ((CrossPlatformInputManager.GetButtonUp(Buttons.Select)/* ||
				CrossPlatformInputManager.GetButtonDown(Buttons.Select)*/)) {
				return true;
			}
			return false;
		}

		protected override void EndMove() {
			CmdDeselectMovingCube(stateManager.SelectedCube.Cube);
		}

		protected override bool IsDisallowed() {
			return overlapManager != null ? overlapManager.HasOverlap() : false;
		}

		protected override void Move() {
			if (cubeRb != null) {
				cubeRb.velocity = cursorCtrl.GetVelocityFromToOffset(
					stateManager.SelectedCube.Cube.transform.position, currOffset);
			}
		}

		[Command]
		private void CmdSelectMovingCube(GameObject cube) {
			RpcSelectMovingCube(cube);
		}

		[ClientRpc]
		void RpcSelectMovingCube(GameObject cube) {
			cube.layer = LayerMask.NameToLayer(Layers.IgnoreRaycast);
			cube.GetComponent<Collider>().isTrigger = true;
			cubeRb = cube.GetComponent<Rigidbody>();
			cubeRb.constraints = RigidbodyConstraints.FreezeRotation;
			cubeNavObstacle = cube.GetComponent<NavMeshObstacle>();
			cubeNavObstacle.enabled = false;

			if (hasAuthority) {
				overlapManager = cube.GetComponent<OverlapManager>();
				cursorCtrl.Refresh();
			}
		}

		[Command]
		private void CmdDeselectMovingCube(GameObject cube) {
			RpcDeselectMovingCube(cube);
		}

		[ClientRpc]
		void RpcDeselectMovingCube(GameObject cube) {
			cube.layer = LayerMask.NameToLayer(Layers.Default);
			cube.GetComponent<Collider>().isTrigger = false;
			cubeRb.constraints = RigidbodyConstraints.None;
			cubeRb.mass = 100;
			cubeRb.velocity = Vector3.zero;
			cubeRb = null;
			ResetOffset();
			cubeNavObstacle.enabled = true;
			cubeNavObstacle = null;
			
			if (hasAuthority) {
				overlapManager = null;
			}
		}

		void ResetOffset() {
			currOffset = cubeOffset + 1;
		}

		bool StuckInDisallow() {
			return
				stateManager.InState(InteractionState.Disallowed) &&
				cubeRb.velocity.magnitude < 2 &&
				overlapManager.HasOverlapWith(cursorCtrl.GetAlignedWith());
		}

		bool IsMoving() {
			return cursorCtrl.IsMoving();
		}

		bool DragWasStartedOnCube(GameObject cube) {
			return dragCubeTarget != null && dragCubeTarget == cube;
		}
	}
}