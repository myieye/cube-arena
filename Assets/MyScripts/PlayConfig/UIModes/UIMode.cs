using System;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.PlayConfig.UIModes {
    public enum UIMode {
        Mouse = 1, HHD1_Camera = 2, HHD2_TouchAndDrag = 3, HHD3_Gestures = 4,
        HMD4_GazeAndClicker = 5, HMD5_Gaze__AirTap_Drag_And_Clicker_Rotate = 6
    }

    static class UIModeHelpers {

        static UIModeHelpers () {
            UIModes = Enum.GetValues (typeof (UIMode)).Cast<UIMode> ().ToList ();
            UIModesForCurrentDevice = UIModes
                //#if !UNITY_EDITOR
                .Where (mode => mode.GetDeviceType ().Equals (DeviceTypeManager.DeviceType))
                //#endif
                .ToList ();
        }

        public static List<UIMode> UIModesForCurrentDevice { get; private set; }
        public static List<UIMode> UIModes { get; private set; }

        public static string GetFriendlyString (this UIMode uiMode) {
            switch (uiMode) {
                default : return uiMode.ToString ()
                    .Replace ("__", ", ")
                    .Replace ("_", " ")
                    .Replace ("And", " + ");
            }
        }

        internal static UIMode DeviceUIModeOrFirst (UIMode uiMode) {
            if (UIModesForCurrentDevice.Contains(uiMode)) {
                return uiMode;
            } else {
                return UIModesForCurrentDevice.First();
            }
        }

        public static DeviceTypeSpec GetDeviceType (this UIMode uiMode) {
            switch (uiMode) {
                case UIMode.HHD1_Camera:
                case UIMode.HHD2_TouchAndDrag:
                case UIMode.HHD3_Gestures:
                    return DeviceTypeSpec.Android;
                case UIMode.HMD4_GazeAndClicker:
                case UIMode.HMD5_Gaze__AirTap_Drag_And_Clicker_Rotate:
                    //case UIMode.HMD6_:
                    return DeviceTypeSpec.HoloLens;
                default:
                    return DeviceTypeSpec.Desktop;
            }
        }
    }
}