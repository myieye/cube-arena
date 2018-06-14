using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Interaction.State;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using CubeArena.Assets.MyScripts.Utils.Settings;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyPrefabs.Cursor {
	[RequireComponent (typeof (CustomARObject))]
	public class CursorController : NetworkBehaviour {

		[SerializeField]
		private float speed;
		[SyncVar]
		private bool showOnNetwork;

		// Components ---
		private Renderer cursorRenderer;
		protected Rigidbody cursorRb;
		private CustomARObject arObj;
		protected InteractionStateManager stateManager;
		// ---

		// Ray casting ---
		protected RaycastHit lastHit;
		private bool prevRaycastSuccess;
		protected bool raycastSuccess;
		public LayerMask raycastLayerMask;
		// ---

		// Screen Points ---
		protected Vector3 screenCenter;
		// ---

		// Movement ---
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
		// ---

		// Sugar ---
		public bool IsActive {
			get {
				return (Raycasting && raycastSuccess) || Translating;
			}
		}
		private bool InARaycastingMode {
			get {
				return !UIModeManager.InCursorMode (CursorMode.Translate);
			}
		}
		private bool InVisibleCursorMode {
			get {
				return Settings.Instance.DebugCursor || !UIModeManager.InTouchMode;
			}
		}
		// ---

		// HMD2
		public Vector3? PointerDirection { get; set; }
		// ---

		// HMD3 ---
		private bool _raycastingEnabled;
		public bool Raycasting {
			get { return _raycastingEnabled || InARaycastingMode; }
			set { _raycastingEnabled = value; }
		}
		public bool Translating {
			get { return !Raycasting; }
		}
		public Vector3 TranslationPosition { get; set; }
		// ---

		public override void OnStartAuthority () {
			Measure.LocalInstance.Cursor = new EnabledComponent<CursorController> (gameObject);
		}

		protected virtual void Start () {
			screenCenter = Camera.main.ViewportToScreenPoint (new Vector3 (0.5f, 0.5f, 0));
			Raycasting = true;

			cursorRenderer = GetComponent<Renderer> ();
			cursorRb = GetComponent<Rigidbody> ();
			arObj = GetComponent<CustomARObject> ();
			stateManager = GetComponent<InteractionStateManager> ();

			if (GetType () == typeof (CursorController) && UIModeManager.InTouchMode) {
				enabled = false;
			}
		}

		public void Refresh () {
			Update ();
			FixedUpdate ();
		}

		protected virtual void FixedUpdate () {
			if (!hasAuthority) return;

			if (IsActive) {
				MoveTowardsTarget ();
				AlignWithTarget ();
			}
		}

		protected virtual void Update () {
			if (!hasAuthority) {
				cursorRenderer.enabled = (ARManager.WorldEnabled &&
					(showOnNetwork || Settings.Instance.DebugCursor));
				return;
			}

			if (Raycasting) {
				UpdateRaycast ();
			} else { // Translating
				raycastSuccess = false;
			}

			cursorRenderer.enabled = ShouldShowCursor ();

			if (showOnNetwork != IsActive) {
				showOnNetwork = IsActive;
				CmdSetShowOnNetwork (showOnNetwork);
			}
		}

		[Command]
		private void CmdSetShowOnNetwork (bool showOnNetwork) {
			this.showOnNetwork = showOnNetwork;
		}

		private void UpdateRaycast () {
			prevRaycastSuccess = raycastSuccess;
			raycastSuccess = SetRayCastHit ();
			var newHit = !prevRaycastSuccess && raycastSuccess;

			if (newHit) {
				TeleportTo (lastHit.point);
			} else if (!raycastSuccess) {
				cursorRb.velocity = Vector3.zero;
			}

			//if (InTouchMode) {
			//AdjustVelocityForTouch ();
			//AdjustOffsetForTouch ();
			//}
		}

		private void TeleportTo (Vector3 position) {
			//cursorRb.position = position;
			cursorRb.velocity = Vector3.zero;
			transform.position = position;
		}

		/*
		void AdjustVelocityForTouch () {
			if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) {
				cursorRb.position = lastHit.point;
			}
		} */

		/*private void AdjustOffsetForTouch () {
			if (Input.touchCount == 0) {
				touchOffsetActive = false;
				currTouchOffset = Vector3.zero;
			} else if (IsMoving (isMovingSensitivity) || touchOffsetActive) {
				touchOffsetActive = true;
				currTouchOffset = Vector2.Lerp (currTouchOffset, touchOffset, Time.deltaTime * touchOffsetSpeed);
			}
		}*/

		public Vector3 GetVelocityFromToOffset (Vector3 from, float offset) {
			var to = TargetPosition;
			if (Raycasting) {
				to += lastHit.normal * offset;
			}
			return GetVelocityFromTo (from, to);
		}

		private void MoveTowardsTarget () {
			cursorRb.velocity = GetVelocityFromTo (cursorRb.position, TargetPosition);
		}

		private void AlignWithTarget () {
			cursorRb.rotation = Quaternion.Lerp (cursorRb.rotation, TargetRotation, Time.deltaTime * speed);
		}

		private bool SetRayCastHit () {
			Vector3? rayCastOrigin = GetRaycastOrigin ();
			return TryCastCursorRay (rayCastOrigin);
		}

		protected virtual Vector3? GetRaycastOrigin () {
			switch (UIModeManager.Instance<UIModeManager> ().CurrentCursorMode) {
				case CursorMode.Mouse:
					return Input.mousePosition;
				case CursorMode.Camera:
				case CursorMode.Pointer:
				case CursorMode.Translate:
				default:
					return screenCenter;
			}
		}

		private bool TryCastCursorRay (Vector3? start) {
			if (start.HasValue) {
				Ray ray;
				if (UIModeManager.InPointerMode && PointerDirection.HasValue) {
					ray = new Ray (Camera.main.ScreenToWorldPoint (start.Value), PointerDirection.Value);
				} else {
					ray = Camera.main.ScreenPointToRay (start.Value);
				}

				return Physics.Raycast (ray, out lastHit, float.MaxValue, raycastLayerMask);
			} else {
				return false;
			}
		}

		public GameObject GetAlignedWith () {
			if (Raycasting && raycastSuccess) {
				if (UIModeManager.InTouchMode && stateManager.HasSelection ()) {
					var cube = stateManager.SelectedCube.Cube;
					return RayUtil.FindGameObjectBelow (cube.transform, raycastLayerMask);
				} else {
					return lastHit.collider.gameObject;
				}
			} else if (Translating) {
				return RayUtil.FindGameObjectBelow (transform, raycastLayerMask);
			} else {
				return null;
			}
		}

		Vector3 GetVelocityFromTo (Vector3 from, Vector3 to) {
			return (to - from) * Time.deltaTime * speed;
		}

		protected virtual bool ShouldShowCursor () {
			return (Settings.Instance.DebugCursor ||
				(Translating || raycastSuccess)) && arObj.ARActive;
		}
	}
}