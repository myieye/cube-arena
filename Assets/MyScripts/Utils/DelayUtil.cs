using System;
using System.Collections;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils {
    public class DelayUtil {
        public static IEnumerator Do(float delay, Action action) {
            yield return new WaitForSeconds (delay);
            action.Invoke();
        }
    }
}