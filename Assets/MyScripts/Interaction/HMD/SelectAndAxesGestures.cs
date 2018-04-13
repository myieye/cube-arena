#if (UNITY_WSA || UNITY_EDITOR)

using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Constants;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.Interaction.HMD
{
    public class SelectAndAxesGestures : MonoBehaviour {

        public float sensitivity = 100f;
        protected GestureRecognizer SelectAxesRecognizer { get; private set; }
        private List<InteractionSourceKind> activeSelectSources;
        private List<InteractionSourceKind> activeAxisSources;
        private GestureRecognizer _activeGestureRecognizer;
        protected GestureRecognizer ActiveGestureRecognizer {
            get {
                return _activeGestureRecognizer;
            } set {
                if (_activeGestureRecognizer == value) return;
                
                if (_activeGestureRecognizer != null) {
                    _activeGestureRecognizer.CancelGestures();
                    _activeGestureRecognizer.StopCapturingGestures();
                }
                if (value != null) {
                    value.StartCapturingGestures();
                }
                _activeGestureRecognizer = value;
            }
        }

        protected virtual void Awake() {
            SelectAxesRecognizer = new GestureRecognizer();
            ActiveGestureRecognizer = SelectAxesRecognizer;
        }

        protected virtual void Start() {
            AddAxisGestures(SelectAxesRecognizer);
            AddSelectGestures(SelectAxesRecognizer);
            SetActiveSelectSources(InteractionSourceKind.Controller);
            SetActiveAxisSources(InteractionSourceKind.Controller);
            ActiveGestureRecognizer = SelectAxesRecognizer;
        }

        void OnDisable() {
            ActiveGestureRecognizer = null;
            //ResetControls();
        }

        void OnDestroy() {
            SelectAxesRecognizer.Dispose();
        }
    
        protected void AddAxisGestures(GestureRecognizer gestureRecognizer) {
            AddRecognizableGestures(gestureRecognizer,
                GestureSettings.NavigationX | GestureSettings.NavigationZ);
            gestureRecognizer.NavigationUpdated += OnNavigationUpdated;
            gestureRecognizer.NavigationCompleted += obj => ResetAxes();
            gestureRecognizer.NavigationCanceled += obj => ResetAxes();
        }

        protected void AddSelectGestures(GestureRecognizer gestureRecognizer) {
            AddRecognizableGestures(gestureRecognizer, GestureSettings.Hold);
            gestureRecognizer.HoldStarted += OnHoldStarted;
            gestureRecognizer.HoldCompleted += OnHoldCompleted;
            gestureRecognizer.HoldCanceled += OnHoldCanceled;
        }

        private void OnNavigationUpdated(NavigationUpdatedEventArgs obj) {
            if (IsActiveAxisSource(obj.source)) {
                var horizontal = obj.normalizedOffset.x * sensitivity;
                var vertical = obj.normalizedOffset.z * sensitivity;
                CrossPlatformInputManager.SetAxis(Axes.Horizontal, horizontal);
                CrossPlatformInputManager.SetAxis(Axes.Vertical, vertical);
            }
        }

        private void OnHoldStarted(HoldStartedEventArgs obj) {
            if (IsActiveSelectionSource(obj.source)) {
                CrossPlatformInputManager.SetButtonDown(Buttons.Select);
            }
        }

        private void OnHoldCompleted(HoldCompletedEventArgs obj) {
            if (IsActiveSelectionSource(obj.source)) {
                CrossPlatformInputManager.SetButtonUp(Buttons.Select);
            }
        }

        private void OnHoldCanceled(HoldCanceledEventArgs obj) {
            if (IsActiveSelectionSource(obj.source)) {
                CrossPlatformInputManager.SetButtonUp(Buttons.Select);
            }
        }

        private void ResetAxes() {
            CrossPlatformInputManager.SetAxis(Axes.Horizontal, 0);
            CrossPlatformInputManager.SetAxis(Axes.Vertical, 0);
        }

        private void ResetControls() {
            ResetAxes();
            CrossPlatformInputManager.SetButtonUp(Buttons.Select);
        }
        protected void AddRecognizableGestures(GestureRecognizer gestureRecognizer, GestureSettings newRgs) {
            var rgs = gestureRecognizer.GetRecognizableGestures();
            gestureRecognizer.SetRecognizableGestures(rgs | newRgs);
        }

        protected void SetActiveSelectSources(params InteractionSourceKind[] sources) {
            activeSelectSources = new List<InteractionSourceKind>(sources);
        }

        protected void SetActiveAxisSources(params InteractionSourceKind[] sources) {
            activeAxisSources = new List<InteractionSourceKind>(sources);
        }

        private bool IsActiveSelectionSource(InteractionSource source) {
            return activeSelectSources.Contains(source.kind);
        }

        private bool IsActiveAxisSource(InteractionSource source) {
            return activeAxisSources.Contains(source.kind);
        }
    }
}
#endif