using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
	[RequireComponent (typeof (BoxCollider))]
	public class KeepInArea : MonoBehaviour {

		void Update () {
			TransformUtil.ClampInArea (transform, GetComponent<BoxCollider> ().size.x / 2);
		}
	}
}