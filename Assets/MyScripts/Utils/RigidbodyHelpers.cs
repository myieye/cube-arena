using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils {
    public static class RigidbodyHelpers {

        public static bool IsMoving (this Rigidbody rigidbody, float minMagnitude = 5) {
            return rigidbody.velocity.magnitude > minMagnitude;
        }
    }
}