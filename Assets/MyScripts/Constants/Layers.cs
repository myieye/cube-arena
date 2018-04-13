using UnityEngine;

namespace CubeArena.Assets.MyScripts.Constants {
    public static class Layers {
        public static LayerMask IgnoreRaycast {
            get {
                return GetLayer ("Ignore Raycast");
            }
        }
        public static LayerMask Default {
            get {
                return GetLayer ("Default");
            }
        }
        public static LayerMask TwoDTranslationPlane {
            get {
                return 1 << GetLayer ("TwoDTranslationPlane");
            }
        }
        public static LayerMask Everything {
            get {
                return GetLayer ("Everything");
            }
        }

        public static LayerMask NotIgnoreRayCast {
            get {
                return ~(1 << IgnoreRaycast); // ignore collisions with layerX
            }
        }

        private static LayerMask GetLayer (string name) {
            return LayerMask.NameToLayer (name);
        }
    }
}