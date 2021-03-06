using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyPrefabs.Cursor {
    public class GestureCubeRotater : AxisCubeRotater {

        /*protected override bool IsStartingRotate() {
            if (UIModeManager.InMode (UIMode.HHD3_Gestures))
			return stateManager.HasSelection() && HasRotationInput() &&
				!stateManager.IsMoving();
		}*/

        protected override Vector3 CalculateRotationTorque () {
            if (!UIModeManager.InUIMode (UIMode.HHD3_Gestures)) {
                savedTorque = base.CalculateRotationTorque ();
            } else if (Input.touchCount < 2) {
                savedTorque = Vector3.zero;
            } else {
                var d1 = Input.GetTouch (0).deltaPosition;
                var d2 = Input.GetTouch (1).deltaPosition;
                var x = Avg (d1.x, d2.x) * speed;
                var y = Avg (d1.y, d2.y) * speed;
                var cameraRelativeTorque = Camera.main.transform.TransformDirection (new Vector3 (y, 0, -x));
                cameraRelativeTorque.y = 0;
                savedTorque = cameraRelativeTorque;
            }
            return savedTorque;
        }

        protected override void StartRotate () {
            if (UIModeManager.InUIMode (UIMode.HMD1_Gaze)) {
                stateManager.LockRotation ();
            }
            base.StartRotate ();
        }

        private float Avg (float a, float b) {
            return (a + b) / 2;
        }
    }
}