#if (UNITY_WSA || UNITY_EDITOR)

using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyPrefabs.Cursor;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.Interaction.HMD
{
    public class SelectAxesAndCursorPointerGestures : SelectAndAxesGestures {

        private GestureRecognizer pointerRecognizer;
        private CursorController cursorCtrl;
        private List<InteractionSourceKind> activePointerSources;
        private InteractionStateManager _stateManager;
        private InteractionStateManager StateManager {
            get {
                if (!_stateManager) {
                    _stateManager = FindObjectOfType<InteractionStateManager>();
                }
                return _stateManager;
            }
        }

        protected override void Awake() {
            base.Awake();
            cursorCtrl = GetComponentInParent<CursorController>();
            pointerRecognizer = new GestureRecognizer();
        }

        protected override void Start() {
            base.Start();
            SetActiveAxisSources(InteractionSourceKind.Controller);
            SetActiveSelectSources(InteractionSourceKind.Hand);
            SetActivePointerSources(InteractionSourceKind.Hand);

            pointerRecognizer.SetRecognizableGestures(GestureSettings.ManipulationTranslate);
            pointerRecognizer.ManipulationStarted += OnManipulationStarted;
            pointerRecognizer.ManipulationUpdated += OnManipulationUpdated;
            pointerRecognizer.ManipulationCompleted += OnManipulationCompleted;
            pointerRecognizer.ManipulationCanceled += OnManipulationCanceled;
        }

        void Update() {
            if (StateManager) {
                if (StateManager.IsMoving() && ActiveGestureRecognizer != pointerRecognizer) {
                    ActiveGestureRecognizer = pointerRecognizer;
                } else if (ActiveGestureRecognizer == pointerRecognizer && !StateManager.IsMoving()) {
                    ActiveGestureRecognizer = SelectAxesRecognizer;
                }
            }
        }

        void OnDestroy() {
            pointerRecognizer.Dispose();
        }

        private void OnManipulationStarted(ManipulationStartedEventArgs obj) {
            Debug.Log("ManipulationRecognizer_ManipulationStarted");
        }

        private void OnManipulationUpdated(ManipulationUpdatedEventArgs obj) {
            if (IsActivePointerSource(obj.source)) {
                cursorCtrl.SetPointerOffset(obj.cumulativeDelta);
            }
        }

        private void OnManipulationCompleted(ManipulationCompletedEventArgs obj) {
            if (IsActivePointerSource(obj.source)) {
                cursorCtrl.SetPointerOffset(Vector3.zero);
            }
        }

        private void OnManipulationCanceled(ManipulationCanceledEventArgs obj) {
            if (IsActivePointerSource(obj.source)) {
                cursorCtrl.SetPointerOffset(Vector3.zero);
            }
        }

        protected void SetActivePointerSources(params InteractionSourceKind[] sources) {
            activePointerSources = new List<InteractionSourceKind>(sources);
        }

        private bool IsActivePointerSource(InteractionSource source) {
            return activePointerSources.Contains(source.kind);
        }
    }
}
#endif