#if (UNITY_WSA || UNITY_EDITOR)

using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.WSA.Input;

namespace CubeArena.Assets.MyScripts.Interaction.HMD.Gestures {
    public class HMD_UI2 : ClickSelectionAndNavigationRotationGestures, IManipulationHandler {

        private float pointerDistance = 10f;

        private Vector3 absoluteRaycastTarget;
        private bool isManipulatingCube;
        private bool resetAfterMove;

        protected override void OnEnable () {
            base.OnEnable ();
            SetEnabledFunctionKind (GestureFunction.Rotate, InteractionSourceKind.Controller);
            SetEnabledFunctionKind (GestureFunction.Select, InteractionSourceKind.Hand);
            SetEnabledFunctionKind (GestureFunction.Point, InteractionSourceKind.Hand);
            Reset ();
        }

        private void Start() {
            Reset ();
        }

        private void Update () {
            if (resetAfterMove && (!StateManager || !StateManager.IsMoving ())) {
                Reset ();
            }
        }

        private void OnDisable () {
            Reset ();
        }

        private void Reset () {
            if (CursorController) {
                CursorController.PointerDirection = null;
            }
            isManipulatingCube = false;
            resetAfterMove = false;
            /*if (StateManager) {
                StateManager.IsManipulating = false;
            }*/
        }

        public override void OnManipulationStarted (ManipulationEventData eventData) {
            if (StateManager.IsHovering () && IsOfEnabledFunctionKind (eventData, GestureFunction.Point)) {
                //StateManager.IsManipulating = true;
                absoluteRaycastTarget = CalcPointerPosition (CursorController.transform.position).ToServer ();
                isManipulatingCube = true;
            }
            base.OnManipulationStarted (eventData);
        }

        public override void OnManipulationUpdated (ManipulationEventData eventData) {
            if (isManipulatingCube && IsOfEnabledFunctionKind (eventData, GestureFunction.Point, probably: true)) {
                var relativeRaycastTarget = absoluteRaycastTarget.ToLocal ();
                relativeRaycastTarget = CalcPointerPosition (
                    //*
                        relativeRaycastTarget +
                    //*/ CursorController.transform.position +
                    (eventData.CumulativeDelta * TransformUtil.LocalRadius));

                //absoluteRaycastTarget = relativeRaycastTarget.ToServer ();

                var direction = relativeRaycastTarget - Camera.main.transform.position;
                CursorController.PointerDirection = direction;
            }
            base.OnManipulationUpdated (eventData);
        }

        public override void OnManipulationCompleted (ManipulationEventData eventData) {
            if (isManipulatingCube && IsOfEnabledFunctionKind (eventData, GestureFunction.Point, probably: true)) {
                resetAfterMove = true;
            }
            base.OnManipulationCompleted (eventData);
        }

        public override void OnManipulationCanceled (ManipulationEventData eventData) {
            if (isManipulatingCube && IsOfEnabledFunctionKind (eventData, GestureFunction.Point, probably: true)) {
                resetAfterMove = true;
            }
            base.OnManipulationCanceled (eventData);
        }

        private Vector3 CalcPointerPosition (Vector3 currPosition) {
            return Camera.main.transform.position + (GetDirectionFromCamera(currPosition) * pointerDistance);
        }

        private Vector3 GetDirectionFromCamera (Vector3 position) {
            var camPosition = Camera.main.transform.position;
            return (position - camPosition).normalized;
        }
    }
}
#endif