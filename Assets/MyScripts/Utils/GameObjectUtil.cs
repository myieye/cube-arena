using System.Linq;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils {
    public static class GameObjectUtil {

        public static T FindObjectOfExactType<T> () where T : Object {
            return GameObject.FindObjectsOfType<T>().First(obj => obj.GetType() == typeof(T));
        }
    }
}