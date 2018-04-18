using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
    public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T> {

        public static T Instance { get; private set; }

        public virtual void Awake () {
            if (Instance) {
                Destroy (gameObject);
            } else {
                Instance = this as T;
            }
        }
    }
}