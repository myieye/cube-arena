using CubeArena.Assets.MyScripts.GameObjects.AR;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Network {

    public class ARRelativeNetworkTransform : RelativeNetworkTransform {

        private bool activated = false;

        // Prevents base.Start
        protected override void Start () { }

        protected override void Update () {
            if (activated && ARManager.WorldEnabled) {
                base.Update ();
            } else if (ARManager.WorldEnabled) {
                Init ();
                activated = true;
            }
        }
    }
}