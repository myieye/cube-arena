using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.Fire;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Utils.Test {
	public class OfflineDynamicFireSource : MonoBehaviour, FireSource {

		public ParticleSystem fire;
		public bool burning;
		private FireCube fireChecker;

		void Start () {
			fireChecker = GetComponentInChildren<FireCube> ();
			burning = fireChecker.CheckBurning ();
		}

		void Update () {
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