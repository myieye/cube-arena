using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using UnityEngine;

namespace CubeArena.Assets.MyPrefabs.Cursor {

    public class GestureCubeSelecter : OverlapCubeSelecter {

        private TapDetecter tapDetecter;

        protected override void Start () {
            base.Start ();
            tapDetecter = FindObjectOfType<TapDetecter> ();
        }

        protected override bool IsDeselecting () {
            if (!UIModeManager.InMode (UIMode.HHD3_Gestures)) {
                return base.IsDeselecting ();
            } else {
                return !selecting && tapDetecter.Tapped &&
                    !stateManager.InStates (InteractionState.Moving, InteractionState.Disallowed);
            }
        }

        protected override bool IsSelecting (out GameObject cube) {
            return base.IsSelecting(out cube);
        }
    }
}