﻿#if (UNITY_WSA || UNITY_EDITOR)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.Utils.Constants;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.Interaction.HMD {
    public class SelectAndAxesGestures : MonoBehaviour, IInputHandler, INavigationHandler {

        public float sensitivity = 100f;
        private Dictionary<InteractionSourceKind, bool> detectedKinds;
        private Dictionary<GestureFunction, InteractionSourceKind> functionToEnabledKind;

        protected virtual void OnEnable () {
      InitDetectedKinds ();
            functionToEnabledKind = new Dictionary<GestureFunction, InteractionSourceKind> ();
            SetEnabledFunctionKind (GestureFunction.Select, InteractionSourceKind.Controller);
            SetEnabledFunctionKind (GestureFunction.Axis, InteractionSourceKind.Controller);
        }

    private void InitDetectedKinds ()
    {
      detectedKinds = new Dictionary<InteractionSourceKind, bool>();
      foreach (var kind in Enum.GetValues(typeof (InteractionSourceKind)).Cast<InteractionSourceKind> ())
      {
        detectedKinds.Add(kind, false);
      }
    }

        public void OnInputDown (InputEventData eventData) {
            var kind = GetAndSaveInteractionSourceKind (eventData);
            if (IsEnabledFunctionKind (kind, GestureFunction.Select)) {
                CrossPlatformInputManager.SetButtonDown (Buttons.Select);
            }
        }

        public void OnInputUp (InputEventData eventData) {
            var kind = GetInteractionSourceKind (eventData, GestureFunction.Select);
            ClearDetectedSourceKind(kind);
            if (IsEnabledFunctionKind (kind, GestureFunction.Select)) {
                CrossPlatformInputManager.SetButtonUp (Buttons.Select);
            }
        }

        void INavigationHandler.OnNavigationStarted (NavigationEventData eventData) {
            // Nothing to do
        }

        void INavigationHandler.OnNavigationUpdated (NavigationEventData eventData) {
            if (IsOfEnabledFunctionKind (eventData, GestureFunction.Axis)) {
                //Debug.Log ("NormalizedOffset: " + eventData.NormalizedOffset);
                var horizontal = eventData.NormalizedOffset.x * sensitivity;
                var vertical = -(eventData.NormalizedOffset.y * sensitivity);
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

        protected void SetEnabledFunctionKind (GestureFunction gestureFunction, InteractionSourceKind kind) {
            if (!functionToEnabledKind.ContainsKey(gestureFunction)) {
                functionToEnabledKind.Add(gestureFunction, kind);
            } else {
                functionToEnabledKind[gestureFunction] = kind;
            }
        }

        protected bool IsOfEnabledFunctionKind (BaseInputEventData eventData, GestureFunction function) {
            var kind = GetInteractionSourceKind(eventData, function);
            return kind.HasValue && functionToEnabledKind.ContainsKey (function) &&
                functionToEnabledKind[function] == (kind.Value);
        }

        private bool IsEnabledFunctionKind (InteractionSourceKind? kind, GestureFunction function) {
            return kind.HasValue && functionToEnabledKind.ContainsKey (function) &&
                functionToEnabledKind[function] == (kind.Value);
        }

        protected InteractionSourceKind? GetAndSaveInteractionSourceKind (BaseInputEventData eventData) {
            var kind = GetInteractionSourceKind (eventData, null);
            if (kind.HasValue) {
                detectedKinds[kind.Value] = true;
            }
            return kind;
        }

        protected void ClearDetectedSourceKind (InteractionSourceKind? kind) {
            if (kind.HasValue) {
                detectedKinds[kind.Value] = false;
            }
        }

        protected InteractionSourceKind? GetInteractionSourceKind (BaseInputEventData eventData, GestureFunction? function) {
            InteractionSourceInfo kind;
            if (eventData.InputSource.TryGetSourceKind (eventData.SourceId, out kind)) {
                return (InteractionSourceKind) kind;
            } else if (function.HasValue && detectedKinds[functionToEnabledKind[function.Value]]) {
                return functionToEnabledKind[function.Value];
            } else {
                Debug.LogError ("Couldn't get source kind");
                return null;
            }
        }
    }
}
#endif