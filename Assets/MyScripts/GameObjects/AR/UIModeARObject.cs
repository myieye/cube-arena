using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.GameObjects.AR {
    public class UIModeARObject : MonoBehaviour, ARObject {

        [SerializeField]
        private UIMode activeUIMode;
        private bool uiModeActive;
        private bool arActive;
        private Renderer rend;

        public void Awake () {
            rend = GetComponent<Renderer> ();
            uiModeActive = false;
            arActive = false;
            Refresh ();
        }

        public void Start () {
            ARManager.Instance.RegisterARObject (this);
        }

        public void OnUIModeChanged (UIMode mode) {
            uiModeActive = activeUIMode.Equals (mode);
            Refresh ();
        }

        public void SetArActive (bool arActive) {
            this.arActive = arActive;
        }

        private void Refresh () {
            rend.enabled = uiModeActive && arActive;
        }
    }
}