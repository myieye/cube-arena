using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
    public abstract class NetworkBehaviourSingleton : NetworkBehaviour {

        private static NetworkBehaviourSingleton _instance;

        public static T Instance<T> () where T : NetworkBehaviourSingleton {
            return _instance as T;
        }

        public virtual void Awake () {
            if (_instance) {
                Destroy (gameObject);
            } else {
                _instance = this;
            }
        }
    }
}