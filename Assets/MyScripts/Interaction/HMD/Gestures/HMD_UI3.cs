#if (UNITY_WSA || UNITY_EDITOR)

using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.WSA.Input;

namespace CubeArena.Assets.MyScripts.Interaction.HMD.Gestures {
    public class HMD_UI3 : ClickSelectNavigationRotateAndManipulationMoveGestures, IManipulationHandler {

        private Vector3 absoluteTarget;

        protected override void OnEnable () {
            base.OnEnable ();
            manipulationFunction = GestureFunction.Translate;
            SetEnabledFunctionKind (GestureFunction.Rotate, InteractionSourceInfo.Controller);
            SetEnabledFunctionKind (GestureFunction.Select, InteractionSourceInfo.Hand);
            SetEnabledFunctionKind (GestureFunction.Translate, InteractionSourceInfo.Hand);
        }

        protected override void Reset (bool disableMoving) {
            base.Reset (disableMoving);
            if (CursorController) {
                CursorController.Raycasting = true;
            }
        }

        public override void OnManipulationStarted (ManipulationEventData eventData) {
            base.OnManipulationStarted (eventData);
            if (isManipulating) {
                CursorController.Raycasting = false;
                absoluteTarget = CursorController.transform.position.ToServer ();
            }
        }

        public override void OnManipulationUpdated (ManipulationEventData eventData) {
            base.OnManipulationUpdated (eventData);
            if (isManipulating) {
                var delta = eventData.CumulativeDelta * TransformUtil.LocalRadius * 2;
                var relativeTarget = absoluteTarget.ToLocal () + delta;

                CursorController.TranslationPosition = relativeTarget;
            }
        }

        public override void OnManipulationCompleted (ManipulationEventData eventData) {
            base.OnManipulationCompleted (eventData);
        }

        public override void OnManipulationCanceled (ManipulationEventData eventData) {
            base.OnManipulationCanceled (eventData);
        }
    }
}
#endif