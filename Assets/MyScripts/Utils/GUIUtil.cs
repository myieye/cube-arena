using UnityEngine;
using UnityEngine.EventSystems;

namespace CubeArena.Assets.MyScripts.Utils {
    public static class GUIUtil {

        public static bool IsOnGUI (int pointerId = -1) {
            return EventSystem.current.IsPointerOverGameObject (pointerId);
        }
    }
}