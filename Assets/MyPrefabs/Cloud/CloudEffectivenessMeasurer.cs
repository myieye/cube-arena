using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyPrefabs.Cloud {
	[RequireComponent (typeof (OverlapManager))]
	public class CloudEffectivenessMeasurer : NetworkBehaviour {

		public Measure PlayerMeasuerer;
		private OverlapManager overlapManager;
		private float overlapTime = 0;
		private float multipleOverlapTime = 0;
		private int numOverlaps = 0;
		private bool measured = false;

		private List<GameObject> prevOverlaps;

		void Start () {
			if (!isServer) return;

			overlapManager = GetComponent<OverlapManager> ();
			prevOverlaps = new List<GameObject> ();
		}

		void Update () {
			if (!isServer) return;

			if (overlapManager.HasOverlap ()) {
				overlapTime += Time.deltaTime;
				multipleOverlapTime += (Time.deltaTime * overlapManager.GetCount ());
				foreach (var obj in overlapManager.touchedObjects) {
					if (!prevOverlaps.Contains (obj)) {
						numOverlaps++;
						prevOverlaps.Add (obj);
					}
				}
			}
		}

		public void Flush () {
			if (!isServer) return;
			
			if (!measured) {
				measured = true;
				PlayerMeasuerer.CloudDestroyed (overlapTime, multipleOverlapTime, numOverlaps);
			}
		}

		void OnDestroy () {
			Flush ();
		}
	}
}