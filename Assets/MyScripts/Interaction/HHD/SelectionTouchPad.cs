using System;
using CubeArena.Assets.MyScripts.Constants;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.Interaction.HHD {
	public class SelectionTouchPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

		bool valid;

		void Update() {
			if (Input.touchCount > 1) {
				valid = false;
				CrossPlatformInputManager.ResetButton(Buttons.Select);
			}

			if (Input.touchCount == 0) {
				valid = true;
			}
		}

		public void OnPointerDown (PointerEventData data) {
			if (valid) {
				CrossPlatformInputManager.SetButtonDown (Buttons.Select);
			}
		}

		public void OnPointerUp (PointerEventData data) {
			if (valid) {
				CrossPlatformInputManager.SetButtonUp (Buttons.Select);
			}
		}
	}
}