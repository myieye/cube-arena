using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.TransformUtils {
    public static class RigidbodyStateUtil {

        public static bool IsValid (this RigidbodyState rbs, bool checkVelocity) {
            var isValid = TransformUtil.IsValid (rbs.position) &&
                TransformUtil.IsValid (rbs.rotation);

            if (checkVelocity) {
                isValid = isValid && TransformUtil.IsValid (rbs.velocity) &&
                    TransformUtil.IsValid (rbs.angularVelocity);
            }

            return isValid;
        }

        public static RigidbodyState BuildRigidbodyState (Transform transform, Rigidbody rb, bool withVelocity) {
            var rbs = new RigidbodyState ();
            rbs.position = transform.position;
            rbs.rotation = transform.rotation;
            if (withVelocity && rb) {
                rbs.velocity = rb.velocity;
                rbs.angularVelocity = rb.angularVelocity;
            }
            return rbs;
        }

        public static void SaveToServerState (ref RigidbodyState rbs, Transform transform, Rigidbody rb, bool withVelocity) {
            rbs.position = TransformUtil.IsCentered ? transform.position : transform.position.ToServer ();
            rbs.rotation = TransformUtil.IsCentered ? transform.rotation : transform.rotation.ToServer ();
            if (withVelocity) {
                rbs.velocity = TransformUtil.IsCentered ? rb.velocity : rb.velocity.ToServerDirection ();
                rbs.angularVelocity = TransformUtil.IsCentered ? rb.angularVelocity : rb.angularVelocity.ToServerDirection ();
            }
        }
    }
}