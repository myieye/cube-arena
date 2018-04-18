using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
	public class StartPositionTracker : MonoBehaviour {

		public Vector3 StartPosition { get; private set; }

		void Start () {
			StartPosition = transform.position;
		}
	}
}