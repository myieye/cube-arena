using System;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;

namespace CubeArena.Assets.MyScripts.PlayConfig.Devices {
    public enum DeviceTypeSpec {
        Android,
        Desktop,
        HoloLens
    }

    public static class DeviceTypeSpecHelpers {

        public static List<DeviceTypeSpec> DeviceTypes { get; private set; }
        public static List<DeviceTypeSpec> TestDeviceTypes { get; private set; }

        static DeviceTypeSpecHelpers () {
            DeviceTypes = Enum.GetValues (typeof (DeviceTypeSpec)).Cast<DeviceTypeSpec> ().ToList ();
            TestDeviceTypes = DeviceTypes.Where (IsTestDeviceType).ToList ();
        }

        private static bool IsTestDeviceType (DeviceTypeSpec deviceType) {
            switch (deviceType) {
                case DeviceTypeSpec.Android:
                case DeviceTypeSpec.HoloLens:
                    return true;
                case DeviceTypeSpec.Desktop:
                default:
                    return false;
            }
        }

        public static bool IsServerDeviceType (this DeviceTypeSpec deviceType) {
            switch (deviceType) {
                case DeviceTypeSpec.Desktop:
                    return true;
                default:
                    return false;
            }
        }
    }
}