using CubeArena.Assets.MyScripts.Logging.DAL.Models;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.PlayConfig.Devices {
    public class DeviceConfig {
        public ConnectedDevice Device { get; set; }
        public UIMode UIMode { get; set; }
    }
}