using UnityEngine;

namespace CubeArena.Assets.MyScripts.PlayConfig.UIModes {
    public interface UIModeChangedListener {
        void OnUIModeChanged (UIMode mode);
        GameObject gameObject { get; }
    }
}