using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Constants {
    public static class Layers {
        public static int Cubes {
            get { return GetLayer ("Cubes"); }
        }
        public static int IgnoreRaycast {
            get { return GetLayer ("Ignore Raycast"); }
        }
        public static int Default {
            get { return GetLayer ("Default"); }
        }
        public static int Everything {
            get { return GetLayer ("Everything"); }
        }
        
        public static LayerMask CubesMask {
            get {
                return GetLayerMask ("Cubes");
            }
        }
        
        public static LayerMask NotIgnoreRayCastMask {
            get {
                return ~(1 << IgnoreRaycast); // ignore collisions with layerX
            }
        }

        public static LayerMask TwoDTranslationPlaneMask {
            get {
                return GetLayerMask ("TwoDTranslationPlane");
            }
        }

        public static LayerMask CubesAndTerrainMask {
            get {
                return GetLayerMask ("Cubes", "Terrain");
            }
        }

        private static int GetLayer (string name) {
            return LayerMask.NameToLayer (name);
        }

        private static LayerMask GetLayerMask (params string[] layers) {
            var mask = 0;
            foreach (var layer in layers) {
                mask |= 1 << GetLayer (layer);
            }
            return mask;
        }
    }
}