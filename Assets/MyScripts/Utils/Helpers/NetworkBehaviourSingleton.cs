using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
    public abstract class NetworkBehaviourSingleton : NetworkBehaviour {

        private static List<NetworkBehaviourSingleton> _instances;

        public static T Instance<T> () where T : NetworkBehaviourSingleton {
            if (_instances == null) return null;
            return _instances.First (i => i.GetType () == typeof (T)) as T;
        }

        public virtual void Awake () {
            if (_instances == null) {
                _instances = new List<NetworkBehaviourSingleton> ();
            }

            if (GetInstance (this)) {
                Destroy (gameObject);
            } else {
                _instances.Add (this);
            }
        }

        private static NetworkBehaviourSingleton GetInstance (NetworkBehaviourSingleton obj) {
            return _instances.FirstOrDefault (i => i.GetType () == obj.GetType ());
        }
    }
}