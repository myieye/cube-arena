using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Utils {
    public static class GameObjectUtil {

        public static T FindObjectOfExactType<T> () where T : Object {
            return GameObject.FindObjectsOfType<T> ().FirstOrDefault (obj => obj.GetType () == typeof (T));
        }

        public static T FindLocalAuthoritativeObject<T> () where T : MonoBehaviour {
            return GameObject.FindObjectsOfType<T> ()
                .FirstOrDefault (obj => obj.GetComponent<NetworkIdentity> ().hasAuthority);
        }
    }
}