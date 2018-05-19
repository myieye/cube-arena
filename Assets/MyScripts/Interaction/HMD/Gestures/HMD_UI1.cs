#if (UNITY_WSA || UNITY_EDITOR)

using CubeArena.Assets.MyScripts.Utils.Constants;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.WSA.Input;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.Interaction.HMD.Gestures {
    public class HMD_UI1 : ClickSelectionAndNavigationRotationGestures {

        [SerializeField]
        [SyncVar]
        private float manipulationThreshold = 200f;
        private float currWait = 0;

        protected override void OnEnable () {
            base.OnEnable ();
            SetEnabledFunctionKind (GestureFunction.Select, InteractionSourceKind.Controller);
            SetEnabledFunctionKind (GestureFunction.Rotate, InteractionSourceKind.Controller);
        }

        /*
        private void Start () {
            Reset ();
        }

        private void OnDisable () {
            Reset ();
        }

        private void Reset () {
            currWait = 0;
            if (StateManager) {
                StateManager.IsManipulating = false;
            }
        }

        private void Update () {
            if (CrossPlatformInputManager.GetButton (Buttons.Select)) {
                currWait += Time.deltaTime;

                if (currWait > manipulationThreshold) {
                    StateManager.IsManipulating = true;
                }
            } else {
                Reset ();
            }
        } */
    }
}
#endif