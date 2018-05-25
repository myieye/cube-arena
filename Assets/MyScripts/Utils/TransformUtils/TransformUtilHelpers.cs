using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.TransformUtils {

    public static class TransformUtilHelpers {

        public static Vector3 Transform (this Vector3 point, TransformDirection direction) {
            return TransformUtil.Transform (direction, point);
        }

        public static Quaternion Transform (this Quaternion rotation, TransformDirection direction) {
            return TransformUtil.Transform (direction, rotation);
        }

        public static RigidbodyState Transform (this Transform transform, TransformDirection direction) {
            return TransformUtil.Transform (direction, transform);
        }

        public static RigidbodyState Transform (this Rigidbody rigidbody, TransformDirection direction) {
            return TransformUtil.Transform (direction, rigidbody);
        }

        public static Vector3 ToServer (this Vector3 point) {
            return TransformUtil.Transform (TransformDirection.LocalToServer, point);
        }

        public static Vector3 ToLocal (this Vector3 point) {
            return TransformUtil.Transform (TransformDirection.ServerToLocal, point);
        }

        public static Quaternion ToServer (this Quaternion rotation) {
            return TransformUtil.Transform (TransformDirection.LocalToServer, rotation);
        }

        public static Quaternion ToLocal (this Quaternion rotation) {
            return TransformUtil.Transform (TransformDirection.ServerToLocal, rotation);
        }

        public static RigidbodyState ToServer (this Transform transform) {
            return TransformUtil.Transform (TransformDirection.LocalToServer, transform);
        }

        public static RigidbodyState ToLocal (this Transform transform) {
            return TransformUtil.Transform (TransformDirection.ServerToLocal, transform);
        }

        public static RigidbodyState ToServer (this Rigidbody rigidbody) {
            return TransformUtil.Transform (TransformDirection.LocalToServer, rigidbody);
        }

        public static RigidbodyState ToLocal (this Rigidbody rigidbody) {
            return TransformUtil.Transform (TransformDirection.ServerToLocal, rigidbody);
        }

        public static Vector3 ToServerDirection (this Vector3 direction) {
            return TransformUtil.TransformVector (TransformDirection.LocalToServer, direction);
        }

        public static Vector3 ToLocalDirection (this Vector3 direction) {
            return TransformUtil.TransformVector (TransformDirection.ServerToLocal, direction);
        }
    }
}