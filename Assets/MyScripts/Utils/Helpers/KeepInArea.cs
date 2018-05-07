using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
	public class KeepInArea : MonoBehaviour {

		void Update () {
			TransformUtil.ClampInArea(transform, GetComponent<Renderer>().bounds.extents.magnitude);
		}
	}
}