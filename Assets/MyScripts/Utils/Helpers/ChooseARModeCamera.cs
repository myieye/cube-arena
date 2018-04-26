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
#if UNITY_EDITOR || (!UNITY_WSA)
			if (Settings.Instance.AREnabled) {
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
			activeCamera.gameObject.SetActive(true);
			inactiveCamera.gameObject.SetActive(false);

			foreach (var canvas in FindObjectsOfType<Canvas> ()) {
				canvas.worldCamera = activeCamera;
			}
		}
	}
}