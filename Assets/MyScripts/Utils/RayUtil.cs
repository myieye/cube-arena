using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils {
    public static class RayUtil {

        private static Vector3 Down {
            get { return TransformUtil.World.up * -1; }
        }

        public static GameObject FindGameObjectBelow (Transform transform, LayerMask layerMask) {
            var start = transform.position + TransformUtil.World.up;
			Ray ray = new Ray (start, Down);
			RaycastHit hit;
			var success = Physics.Raycast (ray, out hit, float.MaxValue, layerMask);
			return success ? hit.collider.gameObject : null;
        }
    }
}