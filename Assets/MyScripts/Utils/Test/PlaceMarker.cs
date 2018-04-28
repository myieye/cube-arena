using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Test {
	public class PlaceMarker : MonoBehaviour {

		[SerializeField]
		private GameObject marker;
		[SerializeField]
		private Vector3 position;

		void Start () {
			Instantiate (marker, position, Quaternion.identity);
		}
	}
}