using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.GameObjects.Fire {
	public class DynamicFireSource : NetworkBehaviour, FireSource {

		public ParticleSystem fire;

		[SyncVar]
		public bool burning;
		private FireCube fireChecker;

		void Start () {
			if (isServer) {
				fireChecker = GetComponentInChildren<FireCube> ();
				burning = fireChecker.CheckBurning ();
			} else {
				Destroy (fireChecker);
			}
		}

		void Update () {
			if (isServer)
				burning = fireChecker.CheckBurning ();

			if (burning && !fire.isPlaying)
				fire.Play ();
			else if (!burning && fire.isPlaying)
				fire.Stop ();
		}

		public bool HasSource () {
			return fireChecker.Source != null;
		}
	}
}