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

        public static List<DeviceTypeSpec> TestDeviceTypes { get; private set; }

        static DeviceTypeSpecHelpers () {
            TestDeviceTypes = UIModeHelpers.TestUIModes
                .Select (UIModeHelpers.GetDeviceType).Distinct ().ToList ();
        }
    }
}