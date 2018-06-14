using System.Collections.Generic;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CubeArena.Assets.MyPrefabs.Cursor {
    public class TouchCursorController : CursorController {

        [SerializeField]
        private float touchOffsetSpeed = 50f;
        [SerializeField]
        private Vector2 touchOffset = new Vector2 (0, 100);
        [SerializeField]
        private float isMovingSensitivity = 10f;

        private Vector2 currTouchOffset;
        private bool touchOffsetActive;
        public Vector2 LastTouch { get; private set; }

        protected override void Start () {
            base.Start ();
            currTouchOffset = Vector2.zero;
            LastTouch = screenCenter;

            if (!UIModeManager.InTouchMode) {
                enabled = false;
            }
        }

        protected override void Update () {
            base.Update ();

            if (!hasAuthority) return;

            AdjustTouchOffset ();

            if (!IsActive) {
                LerpLastTouchToCenter ();
            }

            //if (!raycastSuccess) {
            // Set position on network
            //}
        }

        private void LerpLastTouchToCenter () {
            LastTouch = Vector3.Lerp (LastTouch, screenCenter, Time.deltaTime * 100);
        }

        private void AdjustTouchOffset () {
            if (Input.touchCount == 0) {
                touchOffsetActive = false;
                currTouchOffset = Vector3.zero;
            } else if (cursorRb.IsMoving (isMovingSensitivity) || touchOffsetActive) {
                touchOffsetActive = true;
                currTouchOffset = Vector2.Lerp (currTouchOffset, touchOffset, Time.deltaTime * touchOffsetSpeed);
            }
        }

        protected override Vector3? GetRaycastOrigin () {
            if (Input.touchCount == 1) {
                LastTouch = Input.GetTouch (0).position;
                return LastTouch + currTouchOffset;
            }
            return null;
        }

        protected override bool ShouldShowCursor () {
            return Settings.Instance.DebugCursor;
        }
    }
}