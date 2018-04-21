using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.PlayConfig.Players {
    public class NetworkPlayer {
        public int PlayerId { get; set; }
        public int PlayerNum { get; set; }
        public Color Color { get; set; }
        public DeviceConfig DeviceConfig { get; set; }
        public GameObject PlayerGameObject { get; set; }
        public Transform StartPosition { get; set; }
    }
}