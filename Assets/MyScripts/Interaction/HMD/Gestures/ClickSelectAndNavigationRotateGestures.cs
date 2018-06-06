#if (UNITY_WSA || UNITY_EDITOR)

using CubeArena.Assets.MyPrefabs.Cursor;
using CubeArena.Assets.MyScripts.Interaction.State;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using CubeArena.Assets.MyScripts.Utils.Settings;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.Interaction.HMD.Gestures {
    public class ClickSelectAndNavigationRotateGestures : FunctionBasedGestureHandler, IInputHandler, INavigationHandler, IManipulationHandler, IInputClickHandler {

        private CursorController _cursorCtrl;
        protected CursorController CursorController {
            get {
                if (!_cursorCtrl) {
                    var cursorObject = GameObjectUtil.FindLocalAuthoritativeObject<CursorController> ();
                    if (cursorObject) {
                        _cursorCtrl = cursorObject.GetComponentOfExactType <CursorController> ();
                    }
                }
                return _cursorCtrl;
            }
        }
        private InteractionStateManager _stateManager;
        protected InteractionStateManager StateManager {
            get {
                if (!_stateManager) {
                    _stateManager = GameObjectUtil.FindLocalAuthoritativeObject<InteractionStateManager> ();
                }
                return _stateManager;
            }
        }

        private bool isRotatingCube;

        protected override void OnEnable () {
            base.OnEnable ();
            Reset ();
        }

        private void Start () {
            Reset ();
        }

        private void OnDisable () {
            Reset ();
        }

        private void Reset () {
            isRotatingCube = false;
        }

        public override void OnInputDown (InputEventData eventData) {
            if (IsOfEnabledGestureFunctionKind (eventData, GestureFunction.Select)) {
                CrossPlatformInputManager.SetButtonDown (Buttons.Select);
            }
            base.OnInputDown (eventData);
        }

        public override void OnInputUp (InputEventData eventData) {
            if (IsOfEnabledGestureFunctionKind (eventData, GestureFunction.Select)) {
                CrossPlatformInputManager.SetButtonUp (Buttons.Select);
            }
            base.OnInputUp (eventData);
        }

        public virtual void OnNavigationStarted (NavigationEventData eventData) {
            if (IsDetectedGestureFunction (GestureFunction.Rotate)) {
                isRotatingCube = true;
            }
        }

        public virtual void OnNavigationUpdated (NavigationEventData eventData) {
            if (isRotatingCube) {
                //Debug.Log ("NormalizedOffset: " + eventData.NormalizedOffset);
                var horizontal = eventData.NormalizedOffset.x * Settings.Instance.AxisSensitivity;
                var vertical = -(eventData.NormalizedOffset.y * Settings.Instance.AxisSensitivity);
                CrossPlatformInputManager.SetAxis (Axes.Horizontal, horizontal);
                CrossPlatformInputManager.SetAxis (Axes.Vertical, vertical);
            }
        }

        public virtual void OnNavigationCompleted (NavigationEventData eventData) {
            if (isRotatingCube) {
                ResetAxes ();
            }
        }

        public virtual void OnNavigationCanceled (NavigationEventData eventData) {
            if (isRotatingCube) {
                ResetAxes ();
            }
            CrossPlatformInputManager.SetButtonUp (Buttons.Select);
        }

        public virtual void OnManipulationStarted (ManipulationEventData eventData) { }

        public virtual void OnManipulationUpdated (ManipulationEventData eventData) { }

        public virtual void OnManipulationCompleted (ManipulationEventData eventData) { }

        public virtual void OnManipulationCanceled (ManipulationEventData eventData) {
            CrossPlatformInputManager.SetButtonUp (Buttons.Select);
        }

        public void OnInputClicked (InputClickedEventData eventData) {
            InteractionSourceInfo kind;
            if (eventData.InputSource.TryGetSourceKind (eventData.SourceId, out kind)) {
                Debug.Log (kind);
            }
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

#endif