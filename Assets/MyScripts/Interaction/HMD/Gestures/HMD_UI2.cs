#if (UNITY_WSA || UNITY_EDITOR)

using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.WSA.Input;

namespace CubeArena.Assets.MyScripts.Interaction.HMD.Gestures {
    public class HMD_UI2 : ClickSelectNavigationRotateAndManipulationMoveGestures, IManipulationHandler {

        private float pointerDistance = 10f;
        private Vector3 absoluteRaycastTarget;

        protected override void OnEnable () {
            base.OnEnable ();
            manipulationFunction = GestureFunction.Point;
            SetEnabledFunctionKind (GestureFunction.Rotate, InteractionSourceInfo.Controller);
            SetEnabledFunctionKind (GestureFunction.Select, InteractionSourceInfo.Hand);
            SetEnabledFunctionKind (GestureFunction.Point, InteractionSourceInfo.Hand);
        }

        protected override void Reset (bool disableMoving) {
            base.Reset (disableMoving);
            if (CursorController) {
                CursorController.PointerDirection = null;
            }
        }

        public override void OnManipulationStarted (ManipulationEventData eventData) {
            base.OnManipulationStarted (eventData);
            if (isManipulating) {
                absoluteRaycastTarget = CursorController.transform.position.ToServer ();
            }
        }

        public override void OnManipulationUpdated (ManipulationEventData eventData) {
            base.OnManipulationUpdated (eventData);
            if (isManipulating) {
                var delta = eventData.CumulativeDelta * TransformUtil.LocalRadius * 3.5f;
                var relativeRaycastTarget = absoluteRaycastTarget.ToLocal () + delta;

                var direction = relativeRaycastTarget - Camera.main.transform.position;
                CursorController.PointerDirection = direction;
            }
        }

        public override void OnManipulationCompleted (ManipulationEventData eventData) {
            base.OnManipulationCompleted (eventData);
        }

        public override void OnManipulationCanceled (ManipulationEventData eventData) {
            base.OnManipulationCanceled (eventData);
        }

        private Vector3 CalcPointerPosition (Vector3 currPosition) {
            return Camera.main.transform.position + (GetDirectionFromCamera (currPosition) * pointerDistance);
        }

        private Vector3 GetDirectionFromCamera (Vector3 position) {
            var camPosition = Camera.main.transform.position;
            return (position - camPosition).normalized;
        }
    }
}
#endif