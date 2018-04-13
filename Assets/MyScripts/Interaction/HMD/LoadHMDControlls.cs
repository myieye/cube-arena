using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Constants;
using CubeArena.Assets.MyScripts.Interaction;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Interaction.HMD {
	public class LoadHMDControlls : MonoBehaviour {

		void Start () {
			#if (UNITY_WSA || UNITY_EDITOR)
			var settings = FindObjectOfType<Settings>();
			gameObject.AddComponent<SelectAndAxesGestures>().sensitivity = settings.AxisSensitivity;
			gameObject.AddComponent<SelectAxesAndCursorPointerGestures>().sensitivity = settings.AxisSensitivity;
			#endif
		}
	}
}