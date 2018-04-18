using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
	public class HideIfNotAR : MonoBehaviour {

		void Start () {
			GetComponent<Renderer> ().enabled = !Settings.Instance.AREnabled;
		}
	}
}