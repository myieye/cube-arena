using System;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Network {
    public class PlayerId : NetworkBehaviour {

        private int? _id;

        public int Id {
            get {
                return _id.Value;
            } set {
                if (!_id.HasValue) {
                    _id = value;
                }
            }
        }

        void Start() {
            if (!hasAuthority && !isServer) {
                Destroy(this);
            }
        }
    }
}