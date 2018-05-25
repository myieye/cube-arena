using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
    public static class CameraUtils {

        public static float DistanceFromCamera (Transform transform) {
            return Vector3.Distance (transform.position, Camera.main.transform.position);
        }

        public static float DistanceFromCamera (GameObject gameObject) {
            return DistanceFromCamera (gameObject.transform);
        }
    }
}