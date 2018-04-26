using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
	public class HideIfNotAR : MonoBehaviour, ARObject {

		void Start () {
			ARManager.Instance.RegisterARObject (this);
			GetComponent<Renderer> ().enabled = !Settings.Instance.AREnabled;
		}

		public void SetArActive (bool arEnabled) { }
	}
}