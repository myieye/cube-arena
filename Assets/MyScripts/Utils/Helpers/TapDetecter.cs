using System;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Interaction.Util;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
    public class TapDetecter : MonoBehaviour {

        [SerializeField]
        private float maxDuration;
        private DateTime startTime;
        public bool Tapped { get; private set; }
        public bool Holding { get; private set; }
        private bool valid;

        void Update () {
            
            if (Input.touchCount > 1) {
                valid = false;
            } else if (Input.touchCount == 0) {
                valid = true;
            }

            if (TouchInput.GetPOCDown(0)) {
                startTime = DateTime.Now;
            }

            if (TouchInput.GetPOCUp(0) &&
                (DateTime.Now - startTime).TotalMilliseconds < maxDuration) {
                Tapped = true;
            } else {
                Tapped = false;
            }

            if (TouchInput.GetPOC(0) &&
                (DateTime.Now - startTime).TotalMilliseconds > maxDuration) {
                Holding = true;
            } else {
                Holding = false;
            }

            if (!valid) {
                Tapped = Holding = false;
            }
        }
    }
}