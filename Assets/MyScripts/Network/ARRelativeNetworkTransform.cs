using CubeArena.Assets.MyScripts.GameObjects.AR;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Network {
    
    public class ARRelativeNetworkTransform : RelativeNetworkTransform {

        private bool activated = false;

        protected override void Update () {
            if (activated) {
                base.Update ();
            } else if (ARManager.WorldEnabled) {
                activated = true;
                Init ();
            }
        }
    }
}