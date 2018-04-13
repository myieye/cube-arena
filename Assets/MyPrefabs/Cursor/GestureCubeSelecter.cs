using CubeArena.Assets.MyScripts.Constants;
using CubeArena.Assets.MyScripts.Helpers;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.UI.Mode;
using UnityEngine;

namespace CubeArena.Assets.MyPrefabs.Cursor {

    public class GestureCubeSelecter : OverlapCubeSelecter {

        private TapDetecter tapDetecter;

        protected override void Start () {
            base.Start ();
            tapDetecter = FindObjectOfType<TapDetecter> ();
        }

        protected override bool IsDeselecting () {
            //if (UIModeManager.InMode (UIMode.HHD3_Gestures)) {
            if (!selecting && tapDetecter.Tapped &&
                !stateManager.InStates (InteractionState.Moving, InteractionState.Disallowed)) {
                return true;
            } else {
                return false;
            }
            /*} else {
                return base.IsDeselecting ();
            }*/
        }

        protected override bool IsSelecting (out GameObject cube) {
            if (base.IsSelecting (out cube)) {
                return true;
            } else {
                return false;
            }
        }
    }
}