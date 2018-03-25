using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.AR;
using CubeArena.Assets.MyScripts.Constants;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.UI.Mode;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyPrefabs.Cursor {
	public class CursorController : NetworkBehaviour, ARObject {

		public enum CursorMode { Camera, Mouse, Touch, Pointer }
		public float speed;
		public float isMovingSensitivity = 10f;
		public Vector2 touchOffset = new Vector2 (0, 10);

		private Renderer cursorRenderer;
		private Rigidbody cursorRb;
		private RaycastHit lastHit;
		private bool successfulRaycast;
		private Vector3 screenCenter;
		private Vector2 currTouchOffset = Vector2.zero;
		private Vector3 currPointerOffset = Vector3.zero;
		private UIModeManager uIModeManager;
		private Settings settings;
		private bool _arActive;
		private Vector2 lastTouch;

		private bool InTouchMode {
			get {
				return CursorMode.Touch.Equals (Mode);
			}
		}
		private bool ShowCursor {
			get {
				return !InTouchMode;
			}
		}
		private CursorMode Mode {
			get {
				return uIModeManager.CurrentCursorMode;
			}
		}
		private bool ARActive {
			get {
				return !settings.AREnabled || _arActive;
			}
		}

		void Start () {
			cursorRenderer = GetComponent<Renderer> ();
			cursorRb = GetComponent<Rigidbody> ();
			screenCenter = Camera.main.ViewportToScreenPoint (new Vector3 (0.5f, 0.5f, 0));
			uIModeManager = FindObjectOfType<UIModeManager> ();
			settings = FindObjectOfType<Settings> ();
		}

		public void Refresh () {
			Update ();
			FixedUpdate ();
		}

		void FixedUpdate () {
			if (successfulRaycast) {
				MoveTowardsLastHit ();
				AlignWithLastHit ();
			}
		}

		void Update () {
			SetRayCastHit ();
			ShowHideCursor ();
			if (InTouchMode) {
				AdjustVelocityForTouch ();
				AdjustOffsetForTouch ();
			}
			MeasureUserInteractionArea ();
			//cursorRenderer.enabled = true;
		}

		void AdjustVelocityForTouch () {
			if (!successfulRaycast) {
				cursorRb.velocity = Vector3.zero;
			}
			if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) {
				cursorRb.position = lastHit.point;
			}
		}

		void AdjustOffsetForTouch () {
			if (IsMoving ()) {
				currTouchOffset = touchOffset;
			} else if (Input.touchCount == 0) {
				currTouchOffset = Vector3.zero;
			}
		}

		public Vector3 GetVelocityFromToOffset (Vector3 from, float offset) {
			return GetVelocityFromTo (from, lastHit.point + lastHit.normal * offset);
		}

		void MoveTowardsLastHit () {
			cursorRb.velocity = GetVelocityFromTo (cursorRb.position, lastHit.point);
		}

		void AlignWithLastHit () {
			cursorRb.rotation = Quaternion.Lerp (cursorRb.rotation,
				Quaternion.FromToRotation (Vector3.up, lastHit.normal), Time.deltaTime * speed);
		}

		void SetRayCastHit () {
			if (!hasAuthority) return;

			Vector3? rayCastOrigin = null;
			switch (Mode) {
				case CursorMode.Camera:
					rayCastOrigin = screenCenter;
					break;
				case CursorMode.Mouse:
					rayCastOrigin = Input.mousePosition;
					break;
				case CursorMode.Pointer:
					rayCastOrigin = screenCenter + currPointerOffset;
					break;
				case CursorMode.Touch:
					if (Input.touchCount > 0) {
						lastTouch = Input.GetTouch (0).position;
						rayCastOrigin = lastTouch + currTouchOffset;
					}
					break;
			}

			if (rayCastOrigin.HasValue) {
				Ray ray = Camera.main.ScreenPointToRay (rayCastOrigin.Value);
				successfulRaycast = Physics.Raycast (ray, out lastHit);
			} else {
				successfulRaycast = false;
			}
		}

		internal bool IsUpwardsAligned () {
			var mag = Quaternion.FromToRotation (Vector3.up, lastHit.normal).eulerAngles.magnitude;
			return successfulRaycast && mag < 10;
		}

		internal GameObject GetAlignedWith () {
			return successfulRaycast ? lastHit.collider.gameObject : null;
		}

		Vector3 GetVelocityFromTo (Vector3 from, Vector3 to) {
			return (to - from) * Time.deltaTime * speed;
		}

		public bool IsMoving () {
			return cursorRb.velocity.magnitude > isMovingSensitivity;
		}

		private void ShowHideCursor () {
			cursorRenderer.enabled = ShowCursor && ARActive && successfulRaycast;
		}

		public void SetArActive (bool arActive) {
			this._arActive = arActive;
		}

		public void SetPointerOffset (Vector3 pointerOffset) {
			currPointerOffset = pointerOffset;
		}

		private void MeasureUserInteractionArea () {
			if (successfulRaycast) {
				Measure.UpdateInteractionArea (lastHit.point);
			} else if (!InTouchMode) {
				Measure.UpdateInteractionArea (null);
			} else {
				lastTouch = Vector3.Lerp (lastTouch, screenCenter, Time.deltaTime * 100);
				Ray ray = Camera.main.ScreenPointToRay (lastTouch);
				RaycastHit rayHit;
				if (Physics.Raycast (ray, out rayHit)) {
					Measure.UpdateInteractionArea (rayHit.point);
				} else {
					Measure.UpdateInteractionArea (null);
				}
			}
		}
	}
}