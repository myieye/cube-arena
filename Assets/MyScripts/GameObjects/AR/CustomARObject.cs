using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.GameObjects.AR {
    public class CustomARObject : MonoBehaviour {

        private bool _arActive;
		public virtual bool ARActive {
			get {
				return !Settings.Instance.AREnabled || _arActive;
			} set {
                _arActive = value;
            }
		}

        protected virtual void Start() {
            if (Settings.Instance.AREnabled) {
                ARManager.Instance.RegisterCustomARObject(this);
            }
        }
    }
}