using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
	public class KeepInArea : MonoBehaviour {
		
		[SerializeField]
		private Collider area;
		private float radius;

		void Start () {
			radius = area.bounds.center.x - area.bounds.min.x;
		}

		void Update () {
			var posRelativeToOrigin = Vector3.ClampMagnitude(transform.position - area.bounds.center, radius);
			transform.position = posRelativeToOrigin + area.bounds.center;
		}
	}
}