using System.Linq;
using CubeArena.Assets.MyScripts.Interaction.State;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyPrefabs.Cursor {

    [RequireComponent (typeof (CursorController))]
    public class CursorInteractionMeasurer : NetworkBehaviour {

        protected InteractionStateManager stateManager;
        private EnabledComponent<CursorController> cursor;
        private TouchCursorController TouchCursor {
            get {
                return cursor.Get as TouchCursorController;
            }
        }

        private void Start () {
            stateManager = GetComponent<InteractionStateManager> ();
            cursor = new EnabledComponent<CursorController> (gameObject);
        }

        private void Update () {
            if (!hasAuthority) return;

            MeasureUserInteractionArea ();
        }

        protected virtual void MeasureUserInteractionArea () {
            if (cursor.Get.IsActive) {
                Measure.LocalInstance.UpdateInteractionArea (transform.position);
            } else if (!UIModeManager.InTouchMode) {
                Measure.LocalInstance.UpdateInteractionArea (null);
            } else if (stateManager && stateManager.IsRotating ()) {
                Measure.LocalInstance.UpdateInteractionArea (
                    stateManager.SelectedCube.Cube.transform.position);
            } else {
                Ray ray = Camera.main.ScreenPointToRay (TouchCursor.LastTouch);
                RaycastHit rayHit;
                if (Physics.Raycast (ray, out rayHit)) {
                    Measure.LocalInstance.UpdateInteractionArea (rayHit.point);
                } else {
                    Measure.LocalInstance.UpdateInteractionArea (null);
                }
            }
        }
    }
}