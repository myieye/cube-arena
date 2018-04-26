#if (UNITY_WSA || UNITY_EDITOR)

using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils.Constants;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.Interaction.HMD {
    public class SelectAndAxesGestures : MonoBehaviour, IInputHandler, INavigationHandler {

        public float sensitivity = 100f;
        //protected GestureRecognizer SelectAxesRecognizer { get; private set; }
        private List<InteractionSourceKind> activeSelectSources;
        private List<InteractionSourceKind> activeAxisSources;
        private GestureRecognizer _activeGestureRecognizer;
        protected GestureRecognizer ActiveGestureRecognizer {
            get {
                return _activeGestureRecognizer;
            }
            set {
                if (_activeGestureRecognizer == value) return;

                if (_activeGestureRecognizer != null) {
                    _activeGestureRecognizer.CancelGestures ();
                    _activeGestureRecognizer.StopCapturingGestures ();
                }
                if (value != null) {
                    value.StartCapturingGestures ();
                }
                _activeGestureRecognizer = value;
            }
        }

        protected virtual void Awake () {
            //SelectAxesRecognizer = new GestureRecognizer ();
        }

        protected virtual void OnEnable () {
            //SelectAxesRecognizer.SetRecognizableGestures (GestureSettings.None);

            //AddAxisGestures (SelectAxesRecognizer);
            //AddSelectGestures (SelectAxesRecognizer);

            SetActiveSelectSources (InteractionSourceKind.Other);
            SetActiveAxisSources (InteractionSourceKind.Other);

            //ActiveGestureRecognizer = SelectAxesRecognizer;
        }

        void OnDisable () {
            //ActiveGestureRecognizer = null;
            //ResetControls();
        }

        void OnDestroy () {
            //SelectAxesRecognizer.Dispose ();
        }

        /*
        protected void AddAxisGestures (GestureRecognizer gestureRecognizer) {
            AddRecognizableGestures (gestureRecognizer,
                GestureSettings.NavigationX | GestureSettings.NavigationZ);
            gestureRecognizer.NavigationUpdated += OnNavigationUpdated;
            gestureRecognizer.NavigationCompleted += obj => ResetAxes ();
            gestureRecognizer.NavigationCanceled += obj => ResetAxes ();
        }

        protected void AddSelectGestures (GestureRecognizer gestureRecognizer) {
            AddRecognizableGestures (gestureRecognizer, GestureSettings.Hold);
            gestureRecognizer.HoldStarted += OnHoldStarted;
            gestureRecognizer.HoldCompleted += OnHoldCompleted;
            gestureRecognizer.HoldCanceled += OnHoldCanceled;
        }*/

        /*
        private void OnNavigationUpdated (NavigationUpdatedEventArgs obj) {
            if (IsActiveAxisSourceKind (obj.source.kind)) {
                var horizontal = obj.normalizedOffset.x * sensitivity;
                var vertical = obj.normalizedOffset.y * sensitivity;
                CrossPlatformInputManager.SetAxis (Axes.Horizontal, horizontal);
                CrossPlatformInputManager.SetAxis (Axes.Vertical, vertical);
            }
        }

        private void OnHoldStarted (HoldStartedEventArgs obj) {
            if (IsActiveSelectionSourceKind (obj.source.kind)) {
                CrossPlatformInputManager.SetButtonDown (Buttons.Select);
            }
        }

        private void OnHoldCompleted (HoldCompletedEventArgs obj) {
            if (IsActiveSelectionSourceKind (obj.source.kind)) {
                CrossPlatformInputManager.SetButtonUp (Buttons.Select);
            }
        }

        private void OnHoldCanceled (HoldCanceledEventArgs obj) {
            if (IsActiveSelectionSourceKind (obj.source.kind)) {
                CrossPlatformInputManager.SetButtonUp (Buttons.Select);
            }
        }
         */

        public void OnInputDown (InputEventData eventData) {
            var kind = GetKindFromInputEvent (eventData);
            if (IsActiveSelectionSourceKind (kind)) {
                CrossPlatformInputManager.SetButtonDown (Buttons.Select);
            }
        }

        public void OnInputUp (InputEventData eventData) {
            var kind = GetKindFromInputEvent (eventData);
            if (IsActiveSelectionSourceKind (kind)) {
                CrossPlatformInputManager.SetButtonUp (Buttons.Select);
            }
        }

        void INavigationHandler.OnNavigationStarted (NavigationEventData eventData) {
            // Nothing to do
        }

        void INavigationHandler.OnNavigationUpdated (NavigationEventData eventData) {
            if (IsActiveAxisSourceKind (GetKindFromInputEvent(eventData))) {
                var horizontal = eventData.NormalizedOffset.x * sensitivity;
                var vertical = eventData.NormalizedOffset.y * sensitivity;
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

        /* 
        protected void AddRecognizableGestures (GestureRecognizer gestureRecognizer, GestureSettings newRgs) {
            var rgs = gestureRecognizer.GetRecognizableGestures ();
            gestureRecognizer.SetRecognizableGestures (rgs | newRgs);
        }
        */

        protected void SetActiveSelectSources (params InteractionSourceKind[] sources) {
            activeSelectSources = new List<InteractionSourceKind> (sources);
        }

        protected void SetActiveAxisSources (params InteractionSourceKind[] sources) {
            activeAxisSources = new List<InteractionSourceKind> (sources);
        }

        private bool IsActiveSelectionSourceKind (InteractionSourceKind kind) {
            return activeSelectSources.Contains (kind);
        }

        private bool IsActiveAxisSourceKind (InteractionSourceKind kind) {
            return activeAxisSources.Contains (kind);
        }

        private InteractionSourceKind GetKindFromInputEvent (InputEventData eventData) {
            InteractionSourceInfo kind;
            if (eventData.InputSource.TryGetSourceKind (eventData.SourceId, out kind)) {
                return (InteractionSourceKind) kind;
            } else {
                throw new Exception ("Couldn't get source kind");
            }
        }
    }
}
#endif