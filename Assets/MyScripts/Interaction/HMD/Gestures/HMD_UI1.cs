#if (UNITY_WSA || UNITY_EDITOR)

using CubeArena.Assets.MyScripts.Utils.Constants;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.WSA.Input;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.Interaction.HMD.Gestures {
    public class HMD_UI1 : ClickSelectAndNavigationRotateGestures {

        protected override void OnEnable () {
            base.OnEnable ();
            SetEnabledFunctionKind (GestureFunction.Select, InteractionSourceInfo.Controller);
            SetEnabledFunctionKind (GestureFunction.Rotate, InteractionSourceInfo.Controller);
        }
    }
}
#endif