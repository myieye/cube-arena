using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
	public class HideIfNotAR : CustomARObject {

		protected override void Start () {
			base.Start();
			GetComponent<Renderer> ().enabled = !Settings.Settings.Instance.AREnabled;
		}

		public void SetArActive (bool arEnabled) { }
	}
}