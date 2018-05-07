using CubeArena.Assets.MyPrefabs.Cloud;
using CubeArena.Assets.MyPrefabs.Cursor;
using CubeArena.Assets.MyScripts.Interaction.Abstract;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using ProgressBar;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.GameObjects.Spray {
    public class SprayManager : MonoBehaviourSingleton<SprayManager> {

        private CloudSprayer _sprayer;
        private CloudSprayer Sprayer {
            get {
                if (!_sprayer) {
                    _sprayer = FindObjectOfType<CloudSprayer> ();
                }
                return _sprayer;
            }
        }
		private AbstractSprayToggle _sprayToggle;
		private AbstractSprayToggle SprayToggle {
			get {
				if (!_sprayToggle) {
					_sprayToggle = FindObjectOfType<AbstractSprayToggle> ();
				}
				return _sprayToggle;
			}
		}

        public void ResetSpray () {
            if (Sprayer) {
                Sprayer.Reset ();
            }
            
			foreach (var cloud in FindObjectsOfType<Cloud> ()) {
				Destroy (cloud.gameObject);
			}
            
			if (SprayToggle) {
				SprayToggle.ResetToMove ();
			}
        }
    }
}