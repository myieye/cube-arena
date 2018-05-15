using CubeArena.Assets.MyScripts.Utils.Constants;
using HoloToolkit.Unity.InputModule;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.Interaction.HMD.Gestures {
    public class ClickSelectionAndNavigationRotationGestures : FunctionBasedGestureHandler, IInputHandler, INavigationHandler {

        public override void OnInputDown (InputEventData eventData) {
            base.OnInputDown (eventData);
            var kind = GetInteractionSourceKind (eventData, GestureFunction.Select);
            if (IsEnabledFunctionKind (kind, GestureFunction.Select)) {
                CrossPlatformInputManager.SetButtonDown (Buttons.Select);
            }
        }

        public override void OnInputUp (InputEventData eventData) {
            base.OnInputUp (eventData);
            var kind = GetInteractionSourceKind (eventData, GestureFunction.Select);
            if (IsEnabledFunctionKind (kind, GestureFunction.Select)) {
                CrossPlatformInputManager.SetButtonUp (Buttons.Select);
            }
        }

        void INavigationHandler.OnNavigationStarted (NavigationEventData eventData) {
            // Nothing to do
        }

        void INavigationHandler.OnNavigationUpdated (NavigationEventData eventData) {
            if (IsOfEnabledFunctionKind (eventData, GestureFunction.Rotate)) {
                //Debug.Log ("NormalizedOffset: " + eventData.NormalizedOffset);
                var horizontal = eventData.NormalizedOffset.x * Settings.Instance.AxisSensitivity;
                var vertical = -(eventData.NormalizedOffset.y * Settings.Instance.AxisSensitivity);
                CrossPlatformInputManager.SetAxis (Axes.Horizontal, horizontal);
                CrossPlatformInputManager.SetAxis (Axes.Vertical, vertical);
            }
        }

        void INavigationHandler.OnNavigationCompleted (NavigationEventData eventData) {
            ResetAxes ();
        }

        void INavigationHandler.OnNavigationCanceled (NavigationEventData eventData) {
            ResetAxes ();
        }

        private void ResetAxes () {
            CrossPlatformInputManager.SetAxis (Axes.Horizontal, 0);
            CrossPlatformInputManager.SetAxis (Axes.Vertical, 0);
        }

        private void ResetControls () {
            ResetAxes ();
            CrossPlatformInputManager.SetButtonUp (Buttons.Select);
        }
    }
}