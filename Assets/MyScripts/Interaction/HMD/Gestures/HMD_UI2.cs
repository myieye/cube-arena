#if (UNITY_WSA || UNITY_EDITOR)

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyPrefabs.Cursor;
using CubeArena.Assets.MyScripts.Interaction.State;
using CubeArena.Assets.MyScripts.Utils;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.WSA.Input;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.Interaction.HMD.Gestures {
    public class HMD_UI2 : ClickSelectionAndNavigationRotationGestures, IManipulationHandler {

        protected override void OnEnable () {
            base.OnEnable ();
            SetEnabledFunctionKind (GestureFunction.Rotate, InteractionSourceKind.Controller);
            SetEnabledFunctionKind (GestureFunction.Select, InteractionSourceKind.Hand);
            SetEnabledFunctionKind (GestureFunction.Point, InteractionSourceKind.Hand);
        }

        private CursorController _cursorCtrl;
        private CursorController CursorController {
            get {
                if (!_cursorCtrl) {
                    _cursorCtrl = GameObjectUtil.FindLocalAuthoritativeObject<CursorController> ();
                }
                return _cursorCtrl;
            }
        }
        private InteractionStateManager _stateManager;
        private InteractionStateManager StateManager {
            get {
                if (!_stateManager) {
                    _stateManager = GameObjectUtil.FindLocalAuthoritativeObject<InteractionStateManager> ();
                }
                return _stateManager;
            }
        }

        void IManipulationHandler.OnManipulationStarted (ManipulationEventData eventData) {
            // Nothing to do
        }

        void IManipulationHandler.OnManipulationUpdated (ManipulationEventData eventData) {
            if (IsOfEnabledFunctionKind (eventData, GestureFunction.Point)) {
                CursorController.SetPointerOffset (eventData.CumulativeDelta);
            }
        }

        void IManipulationHandler.OnManipulationCompleted (ManipulationEventData eventData) {
            if (IsOfEnabledFunctionKind (eventData, GestureFunction.Point)) {
                CursorController.SetPointerOffset (Vector3.zero);
            }
        }

        void IManipulationHandler.OnManipulationCanceled (ManipulationEventData eventData) {
            if (IsOfEnabledFunctionKind (eventData, GestureFunction.Point)) {
                CursorController.SetPointerOffset (Vector3.zero);
            }
        }
    }
}
#endif