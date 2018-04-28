using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.GameObjects.AR {
    public class CustomARObject : MonoBehaviour {

        public virtual void Start() {
            if (Settings.Instance.AREnabled) {
                ARManager.Instance.RegisterCustomARObject(this);
            }
        }

        private bool _arActive;
		public virtual bool ARActive {
			get {
				return !Settings.Instance.AREnabled || _arActive;
			} set {
                _arActive = value;
            }
		}
    }
}