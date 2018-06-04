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

        public static T GetComponentOfExactType<T>(this Component component) {
            return component.gameObject.GetComponentOfExactType<T> ();
        }

        public static T GetComponentOfExactType<T>(this GameObject gameObject) {
            return gameObject.GetComponents<T> ().FirstOrDefault (obj => obj.GetType () == typeof (T));
        }
    }
}