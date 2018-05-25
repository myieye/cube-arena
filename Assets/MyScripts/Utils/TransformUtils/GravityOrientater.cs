using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.TransformUtils {
	public class GravityOrientater : MonoBehaviour {

//#if UNITY_WSA && !UNITY_EDITOR
		private float gravityMagnitude;

		void Awake () {
			gravityMagnitude = Physics.gravity.magnitude;
		}

		void Update () {
			Physics.gravity = transform.up * -gravityMagnitude;
		}
//#endif
	}
}