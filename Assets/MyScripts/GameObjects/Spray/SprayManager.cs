using System;
using CubeArena.Assets.MyPrefabs.Cloud;
using CubeArena.Assets.MyPrefabs.Cursor;
using CubeArena.Assets.MyScripts.Interaction.Abstract;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using ProgressBar;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.GameObjects.Spray {
    public class SprayManager : NetworkBehaviourSingleton {

        private CloudSprayer _sprayer;
        private CloudSprayer Sprayer {
            get {
                if (!_sprayer) {
                    _sprayer = GameObjectUtil.FindLocalAuthoritativeObject<CloudSprayer> ();
                }
                return _sprayer;
            }
        }
        private AbstractSprayToggle _sprayToggle;
        private AbstractSprayToggle SprayToggle {
            get {
                if (!_sprayToggle) {
                    _sprayToggle = GameObject.FindObjectOfType<AbstractSprayToggle> ();
                }
                return _sprayToggle;
            }
        }

        public void ResetSpray () {
            RpcResetSpray ();

            foreach (var cloud in FindObjectsOfType<Cloud> ()) {
                NetworkServer.Destroy (cloud.gameObject);
            }
        }

        [ClientRpc]
        public void RpcResetSpray () {
            if (Sprayer) {
                Sprayer.Reset ();
            }

            if (SprayToggle) {
                SprayToggle.ResetToMove ();
            }
        }
    }
}