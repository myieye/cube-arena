using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
	public class AutoStartRound : MonoBehaviour {

#if !UNITY_EDITOR && UNITY_WSA
		private float delay = 5;

		void Update () {
			if (delay > 0) {
				delay -= Time.deltaTime;

				if (Settings.Settings.Instance.AutoStartGame && delay <= 0) {
					FindObjectOfType<NetworkManager> ().StartHost ();
				}
			}
		}
#endif
	}
}