using CubeArena.Assets.MyScripts.Utils;
using UnityEngine;

namespace CubeArena_HoloLens.Assets.MyScripts.Utils.Helpers {
    public static class ComponentEnabler {

        public static void EnableOnly<T> (this GameObject gameObject) where T : Behaviour {
            if (!gameObject) return;

            var comps = gameObject.GetComponents<T> ();

            foreach (var comp in comps) {
                comp.enabled = comp.GetType () == typeof (T);
            }
        }
    }
}