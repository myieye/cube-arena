using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine;

namespace CubeArena.Assets.MyPrefabs.Cubes {
	public class FireUp : MonoBehaviour {

		public float offset = 1.3f;
		private BoxCollider boxCollider;

		void Start () {
			boxCollider = transform.parent.GetComponentInChildren<BoxCollider> ();
		}

		void Update () {
			if (!TransformUtil.IsInitialized) return;

			// Keep at vertical offset and pointed up
			transform.rotation = Quaternion.Lerp (transform.rotation, TransformUtil.World.rotation, Time.deltaTime);
			transform.position = Vector3.Lerp (
				transform.position,
				boxCollider.bounds.center + TransformUtil.World.up * offset,
				Time.deltaTime);
		}
	}
}