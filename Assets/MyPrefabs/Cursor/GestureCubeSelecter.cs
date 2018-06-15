using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using UnityEngine;
using CubeArena.Assets.MyScripts.Interaction.State;

namespace CubeArena.Assets.MyPrefabs.Cursor {

    public class GestureCubeSelecter : OverlapCubeSelecter {

        private CachedComponent<TapDetecter> tapDetecter;

        protected override void Start () {
            base.Start ();
            tapDetecter = new CachedComponent<TapDetecter> ();
        }

        protected override bool IsDeselecting () {
            if (!UIModeManager.InUIMode (UIMode.HHD3_Gestures)) {
                return base.IsDeselecting ();
            } else {
                return !selecting && tapDetecter.Is (t => t.Tapped) &&
                    !stateManager.InStates (InteractionState.Moving, InteractionState.Disallowed);
            }
        }

        protected override bool IsSelecting (out GameObject cube) {
            return base.IsSelecting(out cube);
        }
    }
}