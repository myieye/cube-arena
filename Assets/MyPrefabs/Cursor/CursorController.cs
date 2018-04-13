﻿using System;
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
		private bool successfulRaycast;
		private Vector3 screenCenter;
		private Vector2 currTouchOffset = Vector2.zero;
		private Vector3 currPointerOffset = Vector3.zero;
		private Settings settings;
		private bool _arActive;
		private Vector2 lastTouch;
		public LayerMask raycastLayerMask;
		private bool touchOffsetActive;

		private bool InTouchMode {
			get {
				return CursorMode.Touch.Equals (UIModeManager.Instance.CurrentCursorMode);
			}
		}
		private bool ShowCursor {
			get {
				return Settings.Instance.DebugCursor || !InTouchMode;
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
			if (Input.touchCount == 0) {
				touchOffsetActive = false;
				currTouchOffset = Vector3.zero;
			} else if (IsMoving (isMovingSensitivity) || touchOffsetActive) {
				touchOffsetActive = true;
				currTouchOffset = Vector2.Lerp(currTouchOffset, touchOffset, Time.deltaTime * touchOffsetSpeed);
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

			Vector3? rayCastOrigin = GetRaycastOrigin();
			TryCastRay(rayCastOrigin);
		}

		private Vector3? GetRaycastOrigin () {
			switch (UIModeManager.Instance.CurrentCursorMode) {
				case CursorMode.Camera:
					return screenCenter;
				case CursorMode.Mouse:
					return Input.mousePosition;
				case CursorMode.Pointer:
					return screenCenter + currPointerOffset;
				case CursorMode.Touch:
					if (Input.touchCount > 0) {
						lastTouch = Input.GetTouch (0).position;
						return lastTouch + currTouchOffset;
					}
					break;
			}
			return null;
		}

		private void TryCastRay(Vector3? rayCastOrigin) {
			if (rayCastOrigin.HasValue) {
				Ray ray = Camera.main.ScreenPointToRay (rayCastOrigin.Value);
				successfulRaycast = Physics.Raycast (ray, out lastHit, float.MaxValue, raycastLayerMask);
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

		public bool IsMoving (float minMagnitude = 0) {
			return cursorRb.velocity.magnitude > minMagnitude;
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
				Measure.Instance.UpdateInteractionArea (lastHit.point);
			} else if (!InTouchMode) {
				Measure.Instance.UpdateInteractionArea (null);
			} else {
				lastTouch = Vector3.Lerp (lastTouch, screenCenter, Time.deltaTime * 100);
				Ray ray = Camera.main.ScreenPointToRay (lastTouch);
				RaycastHit rayHit;
				if (Physics.Raycast (ray, out rayHit)) {
					Measure.Instance.UpdateInteractionArea (rayHit.point);
				} else {
					Measure.Instance.UpdateInteractionArea (null);
				}
			}
		}
	}
}