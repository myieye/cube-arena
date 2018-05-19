#if (UNITY_WSA || UNITY_EDITOR)

using CubeArena.Assets.MyScripts.Interaction.HHD;
using CubeArena.Assets.MyScripts.Utils.Constants;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.WSA.Input;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.Interaction.HMD.Gestures {
    public class HMD_SprayToggle : FunctionBasedGestureHandler, IInputClickHandler {

        private HHDSprayToggle sprayToggle;

        private void Start () {
            sprayToggle = FindObjectOfType<HHDSprayToggle> ();
        }

        protected override void OnEnable () {
            base.OnEnable ();
            SetEnabledFunctionKind (GestureFunction.ToggleSpray, InteractionSourceKind.Controller);
        }

        public virtual void OnInputClicked (InputClickedEventData eventData) {
            if (eventData.TapCount == 2 && IsOfEnabledFunctionKind (eventData, GestureFunction.ToggleSpray)) {
                sprayToggle.ToggleState ();
            }
        }
    }
}
#endif