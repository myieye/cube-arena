using System;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.PlayConfig.UIModes {
    public enum UIMode {
        None = 0, Mouse = 1,
        HHD1_Camera = 2, HHD2_Touch = 3, HHD3_Gestures = 4,
        HMD1_Gaze = 5, HMD2_Point = 6, HMD3_Translate = 7
    }

    public static class UIModeHelpers {

        static UIModeHelpers () {
            UIModes = Enum.GetValues (typeof (UIMode)).Cast<UIMode> ().ToList ();
            UIModesForCurrentDevice = UIModes
#if !UNITY_EDITOR
                .Where (mode => mode.IsForDeviceType (DeviceTypeManager.DeviceType))
#endif
                .ToList ();
            ActiveUIModes = UIModes.Where (IsActiveMode).ToList ();
            TestUIModes = UIModes.Where (IsTestMode).ToList ();
        }

        public static List<UIMode> UIModesForCurrentDevice { get; private set; }
        public static List<UIMode> ActiveUIModes { get; private set; }
        public static List<UIMode> UIModes { get; private set; }
        public static List<UIMode> TestUIModes { get; private set; }

        public static string GetFriendlyString (this UIMode uiMode) {
            switch (uiMode) {
                default : return uiMode.ToString ()
                    .Replace ("__", ", ")
                    .Replace ("_", " ")
                    .Replace ("And", " + ");
            }
        }

        internal static UIMode UIModeOrFirstCompatible (UIMode uiMode, DeviceTypeSpec deviceType) {
            var compatibleUIModes = ActiveUIModes.Where (mode => mode.IsForDeviceType (deviceType));
            if (compatibleUIModes.Contains (uiMode)) {
                return uiMode;
            } else {
                return compatibleUIModes.First ();
            }
        }

        public static DeviceTypeSpec GetDeviceType (this UIMode uiMode) {
            switch (uiMode) {
                case UIMode.HHD1_Camera:
                case UIMode.HHD2_Touch:
                case UIMode.HHD3_Gestures:
                    return DeviceTypeSpec.Android;
                case UIMode.HMD1_Gaze:
                case UIMode.HMD2_Point:
                case UIMode.HMD3_Translate:
                    return DeviceTypeSpec.HoloLens;
                case UIMode.None:
                    return DeviceTypeManager.DeviceType;
                case UIMode.Mouse:
                default:
                    return DeviceTypeSpec.Desktop;
            }
        }

        public static bool IsForDeviceType (this UIMode uiMode, DeviceTypeSpec deviceType) {
            return uiMode.GetDeviceType () == deviceType;
        }

        public static bool IsTestMode (this UIMode uiMode) {
            switch (uiMode) {
                case UIMode.None:
                case UIMode.Mouse:
                    return false;
                default:
                    return true;
            }
        }

        public static bool IsActiveMode (this UIMode uiMode) {
            switch (uiMode) {
                case UIMode.None:
                    return false;
                default:
                    return true;
            }
        }
    }
}