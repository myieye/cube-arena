using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.PlayConfig.Devices {
    public class DeviceTypeMessage : MessageBase {
        public DeviceType Type;
        public string Model;
    }
}