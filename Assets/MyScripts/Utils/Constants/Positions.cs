using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Constants {
    public static class Positions {
        public static Vector2 CanvasRightMiddle { get; private set; }
        public static Vector2 CanvasRightBottom { get; private set; }

        static Positions() {
           CanvasRightMiddle = new Vector2(-140, 330);
           CanvasRightBottom = new Vector2(-140, 140);
        }
    }
}