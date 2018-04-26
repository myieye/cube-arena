using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Interaction.HMD {
	public class LoadHMDControlls : MonoBehaviour
  {

		void Awake () {
			#if (UNITY_WSA || UNITY_EDITOR)

			gameObject.AddComponent<SelectAndAxesGestures>().sensitivity =
				Settings.Instance.AxisSensitivity;
			gameObject.AddComponent<SelectAxesAndCursorPointerGestures>().sensitivity =
				Settings.Instance.AxisSensitivity;
				
			#endif
		}
	}
}