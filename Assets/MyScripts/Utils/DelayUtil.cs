using System;
using System.Collections;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils {
    public class DelayUtil {
        public static IEnumerator Do (float delay, Action action) {
            if (delay > 0) {
                yield return new WaitForSeconds (delay);
                action.Invoke ();
            } else {
                action.Invoke ();
            }
        }
    }
}