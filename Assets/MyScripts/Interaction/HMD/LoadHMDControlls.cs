using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Interaction.HMD.Gestures;
using CubeArena.Assets.MyScripts.Utils.Constants;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Interaction.HMD {
	public class LoadHMDControlls : MonoBehaviour {

		void Awake () {
#if (UNITY_WSA || UNITY_EDITOR)
			gameObject.AddComponent<HMD_UI1> ().enabled = false;
			gameObject.AddComponent<HMD_UI2> ().enabled = false;
			gameObject.AddComponent<HMD_UI3> ().enabled = false;
			gameObject.AddComponent<HMD_SprayToggle> ().enabled = false;
#endif
#if UNITY_WSA && !UNITY_EDITOR
			gameObject.AddComponent<SetGlobalListener> ();
#endif
		}
	}
}