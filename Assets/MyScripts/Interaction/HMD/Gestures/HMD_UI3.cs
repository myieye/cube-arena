#if (UNITY_WSA || UNITY_EDITOR)

using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.WSA.Input;

namespace CubeArena.Assets.MyScripts.Interaction.HMD.Gestures {
    public class HMD_UI3 : ClickSelectionAndNavigationRotationGestures, IManipulationHandler {

        private Vector3 absoluteTarget;
        private bool isManipulatingCube;
        private bool resetAfterMove;

        protected override void OnEnable () {
            base.OnEnable ();
            SetEnabledFunctionKind (GestureFunction.Rotate, InteractionSourceKind.Controller);
            SetEnabledFunctionKind (GestureFunction.Select, InteractionSourceKind.Hand);
            SetEnabledFunctionKind (GestureFunction.Translate, InteractionSourceKind.Hand);
            Reset ();
        }

        private void Start () {
            Reset ();
        }

        private void Update () {
            if (resetAfterMove && (!StateManager || !StateManager.IsMoving ())) {
                Reset ();
            }
        }

        protected void OnDisable () {
            Reset ();
        }

        private void Reset () {
            if (CursorController) {
                CursorController.Raycasting = true;
            }
            isManipulatingCube = false;
            resetAfterMove = false;
            /*if (StateManager) {
                StateManager.IsManipulating = false;
            }*/
        }

        public override void OnManipulationStarted (ManipulationEventData eventData) {
            if (StateManager.IsHovering () && IsOfEnabledFunctionKind (eventData, GestureFunction.Translate)) {
                absoluteTarget = CursorController.transform.position.ToServer ();
                CursorController.Raycasting = false;
                isManipulatingCube = true;
                //StateManager.IsManipulating = true;
            }
            base.OnManipulationStarted (eventData);
        }

        public override void OnManipulationUpdated (ManipulationEventData eventData) {
            if (isManipulatingCube && IsOfEnabledFunctionKind (eventData, GestureFunction.Translate, probably : true)) {
                var relativeTarget = absoluteTarget.ToLocal ();
                relativeTarget += (eventData.CumulativeDelta * TransformUtil.LocalRadius);
                //absoluteTarget = relativeTarget.ToServer ();
                CursorController.TranslationPosition = relativeTarget;
            }
            base.OnManipulationUpdated (eventData);
        }

        public override void OnManipulationCompleted (ManipulationEventData eventData) {
            if (isManipulatingCube && IsOfEnabledFunctionKind (eventData, GestureFunction.Translate, probably : true)) {
                resetAfterMove = true;
            }
            base.OnManipulationCompleted (eventData);
        }

        public override void OnManipulationCanceled (ManipulationEventData eventData) {
            if (isManipulatingCube && IsOfEnabledFunctionKind (eventData, GestureFunction.Translate, probably : true)) {
                resetAfterMove = true;
            }
            base.OnManipulationCanceled (eventData);
        }
    }
}
#endif