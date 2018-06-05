using System;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.PlayConfig.Players {
    public class PlayerId : NetworkBehaviour {

        [HideInInspector]
        [SyncVar]
        public int Id;
    }
}