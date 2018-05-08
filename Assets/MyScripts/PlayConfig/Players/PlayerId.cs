using System;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.PlayConfig.Players {
    public class PlayerId : NetworkBehaviour {
        [SyncVar]
        public int Id;
    }
}