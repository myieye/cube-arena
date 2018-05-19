using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Settings;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyPrefabs.Cursor {
	[RequireComponent (typeof (CustomARObject))]
	public class CursorController : NetworkBehaviour {

		public enum CursorMode {
			Camera,
			Mouse,
			Touch,
			Pointer,
			Translate
		}

		public Vector3? Position {
			get {
				return currRaycastSuccess ? (Vector3?) transform.position : null;
			}
		}
		public Vector3? PointerDirection { get; set; }
		private bool _raycastingEnabled;
		public bool Raycasting {
			get { return _raycastingEnabled || InARaycastingMode; }
			set { _raycastingEnabled = value; }
		}
		public bool Translating {
			get { return !Raycasting; }
		}
		public Vector3 TranslationPosition { get; set; }

		[SerializeField]
		private float speed;
		[SerializeField]
		private float touchOffsetSpeed;
		[SerializeField]
		private float isMovingSensitivity = 10f;
		[SerializeField]
		private Vector2 touchOffset = new Vector2 (0, 10);

		private Renderer cursorRenderer;
		private Rigidbody cursorRb;
		private RaycastHit lastHit;
		private bool prevRaycastSuccess;
		private bool currRaycastSuccess;
		private Vector3 screenCenter;
		private Vector2 currTouchOffset = Vector2.zero;
		private Vector2 lastTouch;
		public LayerMask raycastLayerMask;
		private bool touchOffsetActive;
		private CustomARObject arObj;
		private Vector3 TargetPosition {
			get { return Raycasting ? lastHit.point : TranslationPosition; }
		}
		private Quaternion TargetRotation {
			get {
				return Raycasting ?
					Quaternion.FromToRotation (Vector3.up, lastHit.normal) :
					Quaternion.identity.ToLocal ();
			}
		}

		private bool IsActive {
			get {
				return currRaycastSuccess || Translating;
			}
		}
		private bool InTouchMode {
			get {
				return UIModeManager.InCursorMode (CursorMode.Touch);
			}
		}
		private bool InPointerMode {
			get {
				return UIModeManager.InCursorMode (CursorMode.Pointer);
			}
		}
		private bool InARaycastingMode {
			get {
				return !UIModeManager.InCursorMode (CursorMode.Translate);
			}
		}
		private bool ShowCursor {
			get {
				return Settings.Instance.DebugCursor || !InTouchMode;
			}
		}

		void Start () {
			cursorRenderer = GetComponent<Renderer> ();
			cursorRb = GetComponent<Rigidbody> ();
			screenCenter = Camera.main.ViewportToScreenPoint (new Vector3 (0.5f, 0.5f, 0));
			arObj = GetComponent<CustomARObject> ();
		}

		public void Refresh () {
			Update ();
			FixedUpdate ();
		}

		void FixedUpdate () {
			if (!hasAuthority) return;

			if (IsActive) {
				MoveTowardsTarget ();
				AlignWithTarget ();
			}
		}

		void Update () {
			if (!hasAuthority) return;

			if (Raycasting) {
				prevRaycastSuccess = currRaycastSuccess;
				SetRayCastHit ();
				ShowHideCursor ();

				if (!prevRaycastSuccess && currRaycastSuccess) {
					cursorRb.velocity = Vector3.zero;
					cursorRb.position = lastHit.point;
				} else if (!currRaycastSuccess) {
					cursorRb.velocity = Vector3.zero;
				}

				if (InTouchMode) {
					AdjustVelocityForTouch ();
					AdjustOffsetForTouch ();
				}
			} else { // Translating
				currRaycastSuccess = false;
				ShowHideCursor ();
			}

			MeasureUserInteractionArea ();
		}

		void AdjustVelocityForTouch () {
			/*if (!currRaycastSuccess) {
				cursorRb.velocity = Vector3.zero;
			}*/
			if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) {
				cursorRb.position = lastHit.point;
			}
		}

		void AdjustOffsetForTouch () {
			if (Input.touchCount == 0) {
				touchOffsetActive = false;
				currTouchOffset = Vector3.zero;
			} else if (IsMoving (isMovingSensitivity) || touchOffsetActive) {
				touchOffsetActive = true;
				currTouchOffset = Vector2.Lerp (currTouchOffset, touchOffset, Time.deltaTime * touchOffsetSpeed);
			}
		}

		public Vector3 GetVelocityFromToOffset (Vector3 from, float offset) {
			return GetVelocityFromTo (from, lastHit.point + lastHit.normal * offset);
		}

		void MoveTowardsTarget () {
			cursorRb.velocity = GetVelocityFromTo (cursorRb.position, TargetPosition);
		}

		void AlignWithTarget () {
			cursorRb.rotation = Quaternion.Lerp (cursorRb.rotation, TargetRotation, Time.deltaTime * speed);
		}

		void SetRayCastHit () {
			if (!hasAuthority) return;

			Vector3? rayCastOrigin = GetRaycastOrigin ();
			TryCastRay (rayCastOrigin);
		}

		private Vector3? GetRaycastOrigin () {
			switch (UIModeManager.Instance<UIModeManager> ().CurrentCursorMode) {
				case CursorMode.Mouse:
					return Input.mousePosition;
				case CursorMode.Touch:
					if (Input.touchCount > 0) {
						lastTouch = Input.GetTouch (0).position;
						return lastTouch + currTouchOffset;
					}
					return null;
				case CursorMode.Camera:
				case CursorMode.Pointer:
				case CursorMode.Translate:
				default:
					return screenCenter;
			}
		}

		private void TryCastRay (Vector3? rayCastOrigin) {
			if (rayCastOrigin.HasValue) {
				Ray ray;
				if (InPointerMode && PointerDirection.HasValue) {
					ray = new Ray (Camera.main.ScreenToWorldPoint (rayCastOrigin.Value), PointerDirection.Value);
				} else {
					ray = Camera.main.ScreenPointToRay (rayCastOrigin.Value);
				}

				currRaycastSuccess = Physics.Raycast (ray, out lastHit, float.MaxValue, raycastLayerMask);
			} else {
				currRaycastSuccess = false;
			}
		}

		/*
		internal bool IsUpwardsAligned () {
			var mag = Quaternion.FromToRotation (Vector3.up, lastHit.normal).eulerAngles.magnitude;
			return currRaycastSuccess && mag < 10;
		}
		 */

		internal GameObject GetAlignedWith () {
			if (currRaycastSuccess) {
				return lastHit.collider.gameObject;
			} else if (Translating) {
				return GetObjectBelow (TranslationPosition);
			} else {
				return null;
			}
		}

		Vector3 GetVelocityFromTo (Vector3 from, Vector3 to) {
			return (to - from) * Time.deltaTime * speed;
		}

		public bool IsMoving (float minMagnitude = 5) {
			return cursorRb.velocity.magnitude > minMagnitude;
		}

		private void ShowHideCursor () {
			cursorRenderer.enabled = (Translating ||
				(ShowCursor && currRaycastSuccess)) && arObj.ARActive;
		}

		private void MeasureUserInteractionArea () {
			if (IsActive) {
				Measure.LocalInstance.UpdateInteractionArea (TargetPosition);
			} else if (!InTouchMode) {
				Measure.LocalInstance.UpdateInteractionArea (null);
			} else {
				lastTouch = Vector3.Lerp (lastTouch, screenCenter, Time.deltaTime * 100);
				Ray ray = Camera.main.ScreenPointToRay (lastTouch);
				RaycastHit rayHit;
				if (Physics.Raycast (ray, out rayHit)) {
					Measure.LocalInstance.UpdateInteractionArea (rayHit.point);
				} else {
					Measure.LocalInstance.UpdateInteractionArea (null);
				}
			}
		}

		private GameObject GetObjectBelow (Vector3 point) {
			Ray ray = new Ray (point, TransformUtil.World.up * -1);
			RaycastHit hit;
			var success = Physics.Raycast (ray, out hit, float.MaxValue, raycastLayerMask);
			return success ? hit.collider.gameObject : null;
		}
	}
}