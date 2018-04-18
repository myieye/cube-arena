using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
	public class DontDestroyOnLoad : MonoBehaviour {

		public string uniqueId;
		private static List<string> singletonIds = new List<string> ();

		void Awake () {
			var id = string.IsNullOrEmpty (uniqueId) ? gameObject.name : uniqueId;
			if (singletonIds.Contains (id)) {
				Destroy (gameObject);
				return;
			}
			singletonIds.Add (id);
			DontDestroyOnLoad (gameObject);
		}
	}
}