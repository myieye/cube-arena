using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Logging {
	public class LogDeviceType : MonoBehaviour {

		void Start () {

			if (Settings.Instance.LogDeviceInfo) {
	#if UNITY_WSA
				Debug.Log ("FOUND DEVICE: HoloLens");
	#endif
	#if UNITY_ANDROID 
				Debug.Log ("FOUND DEVICE: Android");
	#endif
	#if UNITY_EDITOR
				Debug.Log ("FOUND DEVICE: Unity Editor");
	#endif
				Debug.Log (SystemInfo.deviceType);
				Debug.Log (SystemInfo.operatingSystemFamily);
			}
		}
	}
}