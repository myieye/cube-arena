using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.Interaction.Abstract;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using CubeArena.Assets.MyScripts.Interaction.State;

namespace CubeArena.Assets.MyPrefabs.Cursor {
    public class OverlapCubeSelecter : AbstractCubeSelecter {

        protected bool selecting;
        protected CursorController cursorCtrl;

        protected override void Start () {
            base.Start ();
            cursorCtrl = GetComponent<CursorController> ();
        }

        protected override void Update () {
            base.Update ();
            selecting = selecting && !CrossPlatformInputManager.GetButtonUp (Buttons.Select);
        }

        protected override bool IsDeselecting () {
            return !selecting && CrossPlatformInputManager.GetButtonUp (Buttons.Select) &&
                !stateManager.InStates (InteractionState.Moving, InteractionState.Disallowed);
        }

        protected override bool IsSelecting (out GameObject cube) {
            if (stateManager.IsHovering () && IsPressingSelect () && IsNewSelection ()) {
                cube = stateManager.HoveredCube.Cube;
                return selecting = true;
            }
            cube = null;
            return false;
        }

        protected override bool IsPressingSelect () {
            return CrossPlatformInputManager.GetButtonDown (Buttons.Select);
        }

        private bool IsNewSelection () {
            return !stateManager.IsSelected (stateManager.HoveredCube.Cube);
        }
    }
}