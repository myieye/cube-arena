using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
	public class ChooseARModeCamera : MonoBehaviour {

		[SerializeField]
		private Camera nonArCamera;
		[SerializeField]
		private Camera arCamera;

		void Awake () {
			Camera activeCamera;
			Camera inactiveCamera;
#if UNITY_EDITOR || !UNITY_WSA
			if (Settings.Settings.Instance.AREnabled) {
				activeCamera = arCamera;
				inactiveCamera = nonArCamera;
			} else {
				activeCamera = nonArCamera;
				inactiveCamera = arCamera;
			}
#else
			activeCamera = arCamera;
			inactiveCamera = nonArCamera;
#endif
			activeCamera.gameObject.SetActive (true);
			inactiveCamera.gameObject.SetActive (false);

			foreach (var canvas in Resources.FindObjectsOfTypeAll<Canvas> ()) {
#if UNITY_WSA && !UNITY_EDITOR
				if (!(canvas.renderMode == RenderMode.WorldSpace)) {
					canvas.renderMode = RenderMode.ScreenSpaceCamera;
					canvas.worldCamera = activeCamera;
				}
#else
				canvas.renderMode = RenderMode.ScreenSpaceOverlay;
#endif
			}

		}
	}
}