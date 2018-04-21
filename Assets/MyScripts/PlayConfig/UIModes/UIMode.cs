using System;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.PlayConfig.UIModes {
    public enum UIMode {
        Mouse = 1, HHD1_Camera = 2, HHD2_TouchAndDrag = 3, HHD3_Gestures = 4
#if (UNITY_WSA || UNITY_EDITOR)
        , HMD4_GazeAndClicker = 5, HMD5_Gaze__AirTap_Drag_And_Clicker_Rotate = 6
#endif
    }

    static class UIModeHelpers {

        public static string GetFriendlyString (this UIMode uiMode) {
            switch (uiMode) {
                default : return uiMode.ToString ()
                    .Replace ("__", ", ")
                    .Replace ("_", " ")
                    .Replace ("And", " + ");
            }
        }

        public static DeviceType GetDeviceType (this UIMode uiMode) {
            switch (uiMode) {
                case UIMode.HHD1_Camera:
                case UIMode.HHD2_TouchAndDrag:
                case UIMode.HHD3_Gestures:
                    return DeviceType.Handheld;
#if (UNITY_WSA || UNITY_EDITOR)
                case UIMode.HMD4_GazeAndClicker:
                case UIMode.HMD5_Gaze__AirTap_Drag_And_Clicker_Rotate:
                    //case UIMode.HMD6_:
                    return DeviceType.Desktop;
#endif
                default:
                    return DeviceType.Desktop;
            }
        }
    }
}