#if (UNITY_WSA || UNITY_EDITOR)

using CubeArena.Assets.MyScripts.Interaction.Abstract;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.Utils.Constants;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.WSA.Input;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.Interaction.HMD.Gestures {
    public class HMD_SprayToggle : FunctionBasedGestureHandler, IInputClickHandler {

        private AbstractSprayToggle _sprayToggle;
        private AbstractSprayToggle SprayToggle {
            get {
                if (!_sprayToggle) {
                    _sprayToggle = GameObject.FindObjectOfType<AbstractSprayToggle> ();
                }
                return _sprayToggle;
            }
        }

        public virtual void OnInputClicked (InputClickedEventData eventData) {
            if (eventData.TapCount == 2 && LastInteractionSourceKindWas (InteractionSourceInfo.Controller)) {
                Measure.LocalInstance.CancelTentativeSelection ();
                SprayToggle.ToggleState ();
            }
        }
    }
}
#endif