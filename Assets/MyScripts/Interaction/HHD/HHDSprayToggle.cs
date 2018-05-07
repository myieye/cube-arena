using CubeArena.Assets.MyScripts.Interaction.Abstract;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using UnityEngine;
using UnityEngine.UI;

namespace CubeArena.Assets.MyScripts.Interaction.HHD {
    public class HHDSprayToggle : AbstractSprayToggle {

        private Text toggleText;

        void Awake () {
            if (DeviceTypeManager.IsDeviceType (DeviceTypeSpec.HoloLens)) {
                Destroy (this);
                return;
            }

            toggleText = GetComponentInChildren<Text> ();
            toggleText.text = Utils.Constants.Text.Move;
        }

        public override void ToggleState () {
            base.ToggleState ();
            if (StateManager.IsSpraying ()) {
                toggleText.text = Utils.Constants.Text.Spray;
            } else {
                toggleText.text = Utils.Constants.Text.Move;
            }
        }

        public override void ResetToMove () {
            base.ResetToMove ();
            toggleText.text = Utils.Constants.Text.Move;
        }
    }
}