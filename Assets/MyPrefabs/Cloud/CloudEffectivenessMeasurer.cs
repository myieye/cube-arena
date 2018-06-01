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

		public Measure PlayerMeasuerer { get; set; }

		private OverlapManager overlapManager;
		private float overlapTime = 0;
		private float multipleOverlapTime = 0;
		private int numOverlaps = 0;
		private bool measured = false;

		private List<GameObject> prevOverlaps;

		[ServerCallback]
		void Start () {
			overlapManager = GetComponent<OverlapManager> ();
			prevOverlaps = new List<GameObject> ();
		}

		[ServerCallback]
		void Update () {
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

		[ServerCallback]
		public void Flush () {
			if (!measured) {
				measured = true;
				PlayerMeasuerer.CloudDestroyed (overlapTime, multipleOverlapTime, numOverlaps);
			}
		}
	}
}