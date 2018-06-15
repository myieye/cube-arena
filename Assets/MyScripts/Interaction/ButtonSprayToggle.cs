using CubeArena.Assets.MyScripts.Interaction.Abstract;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using UnityEngine;
using UnityEngine.UI;

namespace CubeArena.Assets.MyScripts.Interaction {
    public class ButtonSprayToggle : AbstractSprayToggle {

        private Text toggleText;

        void Awake () {
            toggleText = GetComponentInChildren<Text> ();
            toggleText.text = Utils.Constants.Text.Move;
#if UNITY_WSA && !UNITY_EDITOR
            GetComponent<RectTransform> ().sizeDelta = new Vector2 (150, 100);
#endif
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