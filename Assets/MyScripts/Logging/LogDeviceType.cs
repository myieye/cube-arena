using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogDeviceType : MonoBehaviour {

	void Start () {
		var deviceMsg = "FOUND DEVICE: ";
		#if UNITY_WSA
		deviceMsg += "HoloLens";
		#elif UNITY_ANDROID 
		deviceMsg += "Android";
		#elif UNITY_EDITOR
		deviceMsg += "Unity Editor";
		#endif
		Debug.Log(deviceMsg);
	}
}
