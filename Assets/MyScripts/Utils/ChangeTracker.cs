using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
using Vuforia;

namespace CubeArena.Assets.MyScripts.Utils
{
	public class ChangeTracker : DefaultTrackableEventHandler {

		private Terrain terrain;

		override protected void Start() {
			terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
			terrain.enabled = false;
		}
		
		override protected void OnTrackingFound() {
			Debug.Log("Found");
			base.OnTrackingFound();
			Debug.Log("Setting Enabled");
			terrain.enabled = true;
		}
		
		override protected void OnTrackingLost() {
			base.OnTrackingLost();
			terrain.enabled = false;
		}
	}
}
*/