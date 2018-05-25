#if (UNITY_WSA || UNITY_EDITOR)

using System;
using System.Collections.Generic;
using System.Linq;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.WSA.Input;

namespace CubeArena.Assets.MyScripts.Interaction.HMD.Gestures {
    public abstract class FunctionBasedGestureHandler : MonoBehaviour, IInputHandler {
        private Dictionary<InteractionSourceInfo, bool> detectedKinds;
        private Dictionary<GestureFunction, InteractionSourceInfo> functionToEnabledKind;
        private InteractionSourceInfo? lastInteractionSourceKind;

        protected virtual void OnEnable () {
            InitDetectedKinds ();
            functionToEnabledKind = new Dictionary<GestureFunction, InteractionSourceInfo> ();
        }

        protected bool LastInteractionSourceKindWas (InteractionSourceInfo kind) {
            return lastInteractionSourceKind.HasValue &&
                lastInteractionSourceKind.Value == kind;
        }

        private void InitDetectedKinds () {
            detectedKinds = new Dictionary<InteractionSourceInfo, bool> ();
            foreach (var kind in Enum.GetValues (typeof (InteractionSourceInfo)).Cast<InteractionSourceInfo> ()) {
                detectedKinds.Add (kind, false);
            }
        }

        public virtual void OnInputDown (InputEventData eventData) {
            GetAndSaveInteractionSourceKind (eventData);
        }

        public virtual void OnInputUp (InputEventData eventData) {
            var kind = GetInteractionSourceKind (eventData);
            ClearDetectedSourceKind (kind);
        }

        protected void SetEnabledFunctionKind (GestureFunction gestureFunction, InteractionSourceInfo kind) {
            if (!functionToEnabledKind.ContainsKey (gestureFunction)) {
                functionToEnabledKind.Add (gestureFunction, kind);
            } else {
                functionToEnabledKind[gestureFunction] = kind;
            }
        }

        protected bool IsOfEnabledGestureFunctionKind (BaseInputEventData eventData, GestureFunction function) {
            var kind = GetInteractionSourceKind (eventData);
            return IsEnabledFunctionKind (kind, function);
        }

        protected bool IsDetectedGestureFunction (GestureFunction function) {
            return functionToEnabledKind.ContainsKey (function) &&
                detectedKinds[functionToEnabledKind[function]];
        }

        private bool IsEnabledFunctionKind (InteractionSourceInfo? kind, GestureFunction function) {
            return kind.HasValue && functionToEnabledKind.ContainsKey (function) &&
                functionToEnabledKind[function] == kind.Value;
        }

        private InteractionSourceInfo? GetAndSaveInteractionSourceKind (BaseInputEventData eventData) {
            var kind = GetInteractionSourceKind (eventData);
            if (kind.HasValue) {
                detectedKinds[kind.Value] = true;
                lastInteractionSourceKind = kind.Value;
            }
            return kind;
        }

        private void ClearDetectedSourceKind (InteractionSourceInfo? kind) {
            if (kind.HasValue) {
                detectedKinds[kind.Value] = false;
            }
        }

        private InteractionSourceInfo? GetInteractionSourceKind (BaseInputEventData eventData) {
            InteractionSourceInfo kind;
            if (eventData.InputSource.TryGetSourceKind (eventData.SourceId, out kind)) {
                return kind;
            } else {
                Debug.LogError ("Couldn't get source kind");
                return null;
            }
        }
    }
}

#endif