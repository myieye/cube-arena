using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Interaction;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Interaction.HMD {
	public class LoadHMDControlls : MonoBehaviour {

		void Start () {
			#if (UNITY_WSA || UNITY_EDITOR)
			gameObject.AddComponent<SelectAndAxesGestures>().sensitivity =
				Settings.Instance.AxisSensitivity;
			gameObject.AddComponent<SelectAxesAndCursorPointerGestures>().sensitivity =
				Settings.Instance.AxisSensitivity;
			#endif
		}
	}
}