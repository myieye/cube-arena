#if (UNITY_WSA || UNITY_EDITOR)

using System;
using System.Linq;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace CubeArena.Assets.MyScripts.Interaction.HMD.Gestures {
    public abstract class FunctionBasedGestureHandler : MonoBehaviour {
        private Dictionary<InteractionSourceKind, bool> detectedKinds;
        private Dictionary<GestureFunction, InteractionSourceKind> functionToEnabledKind;

        protected virtual void OnEnable () {
            InitDetectedKinds ();
            functionToEnabledKind = new Dictionary<GestureFunction, InteractionSourceKind> ();
        }

        private void InitDetectedKinds () {
            detectedKinds = new Dictionary<InteractionSourceKind, bool> ();
            foreach (var kind in Enum.GetValues (typeof (InteractionSourceKind)).Cast<InteractionSourceKind> ()) {
                detectedKinds.Add (kind, false);
            }
        }

        public virtual void OnInputDown (InputEventData eventData) {
            GetAndSaveInteractionSourceKind (eventData);
        }

        public virtual void OnInputUp (InputEventData eventData) {
            var kind = GetInteractionSourceKind (eventData, GestureFunction.Select);
            ClearDetectedSourceKind (kind);
        }

        protected void SetEnabledFunctionKind (GestureFunction gestureFunction, InteractionSourceKind kind) {
            if (!functionToEnabledKind.ContainsKey (gestureFunction)) {
                functionToEnabledKind.Add (gestureFunction, kind);
            } else {
                functionToEnabledKind[gestureFunction] = kind;
            }
        }

        protected bool IsOfEnabledFunctionKind (BaseInputEventData eventData, GestureFunction function) {
            var kind = GetInteractionSourceKind (eventData, function);
            return kind.HasValue && functionToEnabledKind.ContainsKey (function) &&
                functionToEnabledKind[function] == (kind.Value);
        }

        protected bool IsEnabledFunctionKind (InteractionSourceKind? kind, GestureFunction function) {
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