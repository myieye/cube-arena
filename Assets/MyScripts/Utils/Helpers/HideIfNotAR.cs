using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
	public class HideIfNotAR : CustomARObject {

		public override void Start () {
			base.Start();
			GetComponent<Renderer> ().enabled = !Settings.Instance.AREnabled;
		}

		public void SetArActive (bool arEnabled) { }
	}
}