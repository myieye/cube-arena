using System;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using UnityEngine.Networking;

namespace CubeArena.Assets.Tests.MyScripts.Utils.Devices {
    public static class DeviceTestUtil {
        public static Dictionary<DeviceTypeSpec, List<ConnectedDevice>> GetDevicesByType (
            int numAndroidDevices = 0, int numHoloLensDevices = 0, int numDesktopDevices = 0) {
            var devicesByType = new Dictionary<DeviceTypeSpec, List<ConnectedDevice>> ();

            int id = 1;

            var androidDevices = new List<ConnectedDevice> ();
            for (int i = 0; i < numAndroidDevices; i++) {
                androidDevices.Add (Device (id++, DeviceTypeSpec.Android));
            }

            var holoLensDevices = new List<ConnectedDevice> ();
            for (int i = 0; i < numHoloLensDevices; i++) {
                holoLensDevices.Add (Device (id++, DeviceTypeSpec.HoloLens));
            }

            var desktopDevices = new List<ConnectedDevice> ();
            for (int i = 0; i < numDesktopDevices; i++) {
                desktopDevices.Add (Device (id++, DeviceTypeSpec.Desktop));
            }

            devicesByType.Add (DeviceTypeSpec.Android, androidDevices);
            devicesByType.Add (DeviceTypeSpec.HoloLens, holoLensDevices);
            devicesByType.Add (DeviceTypeSpec.Desktop, desktopDevices);

            return devicesByType;
        }

        public static ConnectedDevice Device (int id, DeviceTypeSpec deviceType) {
            var conn = new NetworkConnection { connectionId = id, address = "192.168.1." + id };
            return new ConnectedDevice (conn, (short) id, deviceType, deviceType.ToString ()) { Id = id };
        }

        public static void RegisterDevices (DeviceManager deviceManager, Dictionary<DeviceTypeSpec, List<ConnectedDevice>> devicesByType) {
            foreach (var device in devicesByType.Values.SelectMany (x => x).ToList ()) {
                deviceManager.RegisterConnectedDevice (device);
            }
        }
    }
}