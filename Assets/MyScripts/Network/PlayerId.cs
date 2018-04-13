using System;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Network {
    public class PlayerId : NetworkBehaviour {

        public int Id { get; [Server] set; }

        void Start() {
            if (!hasAuthority && !isServer) {
                Destroy(this);
            }
        }
    }
}