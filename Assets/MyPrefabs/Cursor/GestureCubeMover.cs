using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyPrefabs.Cursor;
using CubeArena.Assets.MyScripts.Constants;
using CubeArena.Assets.MyScripts.Helpers;
using CubeArena.Assets.MyScripts.Interaction.Util;
using CubeArena.Assets.MyScripts.UI.Mode;
using UnityEngine;

namespace CubeArena.Assets.MyPrefabs.Cursor {

	public class GestureCubeMover : RaycastCubeMover {

		private enum MoveState { None, XZ, Y }

		[SerializeField]
		private float speed;
		private TapDetecter tapDetecter;
		private GameObject translationPlane;
		private MoveState moveState;
		private Vector2? prevYPoint;

		private bool InGestureMode {
			get {
				return UIModeManager.InMode (UIMode.HHD3_Gestures);
			}
		}

		protected override void Start () {
			base.Start ();
			tapDetecter = FindObjectOfType<TapDetecter> ();
			translationPlane = GameObject.FindWithTag (Tags.TwoDTranslationPlane);
			moveState = MoveState.None;
			prevYPoint = null;
		}

		protected override void Update () {
			if (InGestureMode) {
				GameObject cube;

				if (TouchInput.NoPOCs ()) {
					prevYPoint = null;
					SetMoveState(MoveState.None);
				}

				if (stateManager.IsMoving()) {
					if (StartingYMove (out cube)) {
						SetMoveState (MoveState.Y);
					} else if (!moveState.Equals (MoveState.XZ) && base.IsStartingMove (out cube)) {
						SetMoveState (MoveState.XZ);
					}
				}
			}

			base.Update ();
		}

		protected override bool IsStartingMove (out GameObject cube) {
			cube = null;
			if (InGestureMode) {
				return Input.touchCount < 2 &&
					(StartingYMove (out cube) || base.IsStartingMove (out cube));
			} else {
				return base.IsStartingMove(out cube);
			}
		}

		protected override bool IsEndingMove () {
			return (InGestureMode && tapDetecter.Tapped) || (!InGestureMode && base.IsEndingMove ());
		}

		protected override void Move () {
			if (!InGestureMode || moveState.Equals (MoveState.XZ)) {
				base.Move ();
			} else if (moveState.Equals (MoveState.Y) && TouchInput.HasSinglePOC () && cubeRb != null) {
				var currYPoint = TouchInput.GetPOCPosition (0);
				if (prevYPoint.HasValue) {
					cubeRb.velocity = CalcYVelocity (prevYPoint.Value, currYPoint);
				}
				prevYPoint = currYPoint;
			}
		}

		protected override void StartMove (GameObject cube) {
			base.StartMove (cube);
			if (StartingYMove (out cube)) {
				SetMoveState (MoveState.Y);
				prevYPoint = TouchInput.GetPOCPosition (0);
			} else if (InGestureMode) {
				SetMoveState (MoveState.XZ);
			}
		}

		protected override void EndMove () {
			base.EndMove ();
			SetMoveState (MoveState.None);
		}

		private Vector3 CalcYVelocity (Vector2 from, Vector2 to) {
			return Vector3.up * (to.y - from.y) * Time.deltaTime * speed * GetDistanceFromCamera ();
		}

		private float GetDistanceFromCamera () {
			return Vector3.Distance (cubeRb.transform.position, Camera.main.transform.position);
		}

		private bool StartingYMove (out GameObject cube) {
			cube = stateManager.HasSelection () ? stateManager.SelectedCube.Cube : null;
			return InGestureMode && !moveState.Equals(MoveState.Y) && stateManager.HasSelection () &&
				HoldingSinglePOC () && dragCubeTarget == null;
		}

		private bool HoldingSinglePOC () {
			return TouchInput.HasSinglePOC () && tapDetecter.Holding;
		}

		private void SetMoveState (MoveState moveState) {
			if (moveState.Equals(this.moveState)) return;

			if (moveState.Equals (MoveState.XZ)) {
				cursorCtrl.raycastLayerMask = Layers.TwoDTranslationPlane;
				translationPlane.transform.position = Vector3.up * cubeRb.transform.position.y;
			} else {
				cursorCtrl.raycastLayerMask = Layers.NotIgnoreRayCast;
				translationPlane.transform.position = Vector3.zero;
				if (cubeRb != null) {
					cubeRb.gameObject.layer = Layers.Default;
				}
			}

			this.moveState = moveState;
		}

		public override void OnCubeDeselected (GameObject cube) {
			SetMoveState (MoveState.None);
			base.OnCubeDeselected (cube);
		}
	}
}