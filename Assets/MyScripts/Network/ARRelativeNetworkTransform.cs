using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Network {

    public class ARRelativeNetworkTransform : RelativeNetworkTransform {

        // Prevents base.Start
        protected override void Start () { }

        protected override void Update () {
            if (isInitialized && ARManager.WorldEnabled) {
                base.Update ();
            } else if (ARManager.WorldEnabled) {
                Init ();
            }
        }
    }
}