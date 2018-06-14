using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.GameObjects.AR {
    public class UIModeARObject : CustomARObject, UIModeChangedListener {

        [SerializeField]
        private UIMode activeUIMode;
        [SerializeField]
        private bool visible;

        private bool uiModeActive;
        private Renderer rend;
        private Collider coll;
        public override bool ARActive {
            set {
                base.ARActive = value;
                Refresh ();
            }
        }

        public void Awake () {
            rend = GetComponent<Renderer> ();
            coll = GetComponent<Collider> ();
            uiModeActive = false;
            ARActive = false;
            Refresh ();
        }

        protected override void Start () {
            base.Start ();
            UIModeManager.RegisterUIModeChangedListener (this);
        }

        public void OnUIModeChanged (UIMode mode) {
            uiModeActive = activeUIMode.Equals (mode);
            Refresh ();
        }

        private void Refresh () {
            if (rend) {
                rend.enabled = visible && uiModeActive && ARActive;
            }
            if (coll) {
                coll.enabled = uiModeActive && ARActive;
            }
        }

        private void OnDestroy () {
            UIModeManager.UnregisterUIModeChangedListener (this);
        }
    }
}