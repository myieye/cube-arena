using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Network {
    public class NetworkPlayer {
        public NetworkConnection Connection { get; set; }
        public short PlayerControllerId { get; set; }
        public int PlayerId { get; set; }
        public GameObject Player { get; set; }
    }
}