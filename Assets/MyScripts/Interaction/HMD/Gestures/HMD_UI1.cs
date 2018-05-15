#if (UNITY_WSA || UNITY_EDITOR)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.Utils.Constants;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.Interaction.HMD.Gestures {
    public class HMD_UI1 : ClickSelectionAndNavigationRotationGestures {
        protected override void OnEnable () {
            base.OnEnable ();
            SetEnabledFunctionKind (GestureFunction.Select, InteractionSourceKind.Controller);
            SetEnabledFunctionKind (GestureFunction.Rotate, InteractionSourceKind.Controller);
        }
    }
}
#endif