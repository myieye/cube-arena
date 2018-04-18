using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.PlayConfig.Devices {
    public class DeviceConfig {

        public DeviceConfig (ConnectedDevice device, UIMode uIMode) {
            this.Device = device;
            this.UIMode = uIMode;
        }

        public ConnectedDevice Device { get; private set; }
        public UIMode UIMode { get; private set; }
    }
}