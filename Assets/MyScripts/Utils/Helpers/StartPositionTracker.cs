using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Logging;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
	public class StartPositionTracker : NetworkBehaviour {

		public Vector3 StartPosition { get; private set; }

		void Start () {
			StartPosition = transform.position;
		}

		public override void OnStartAuthority () {
			base.OnStartAuthority ();
			Measure.LocalInstance.StartPosition = StartPosition;
		}
	}
}