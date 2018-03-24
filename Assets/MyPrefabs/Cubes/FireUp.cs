using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyPrefabs.Cubes
{
	public class FireUp : MonoBehaviour {

		public float offset = 1.3f;
		private BoxCollider boxCollider;

		void Start () {
			boxCollider = transform.parent.GetComponentInChildren<BoxCollider>();
		}
		
		void Update () {
			// Keep at vertical offset and pointed up
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime);
			transform.position = Vector3.Lerp(
				transform.position,
				boxCollider.bounds.center + new Vector3(0, offset, 0),
				Time.deltaTime);
		}
	}
}