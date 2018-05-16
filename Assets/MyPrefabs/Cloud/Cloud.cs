using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Colors;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyPrefabs.Cloud {

	[RequireComponent (typeof (Renderer), typeof (ARObject))]
	public class Cloud : NetworkBehaviour {

		[SerializeField]
		private float lifetime;
		private DateTime createdAt;
		private Renderer rend;
		private float maxA;
		private Colourer colourer;

		void Start () {
			// Server Code ------------------------
			if (!isServer) return;

			colourer = GetComponent<Colourer> ();
			maxA = GetComponent<Renderer> ().material.color.a;
			createdAt = DateTime.Now;
		}

		void Update () {
			// Server Code ------------------------
			if (!isServer) return;

			var age = (DateTime.Now - createdAt).TotalMilliseconds;
			var color = colourer.color;
			color.a = Mathf.Min ((float) (1 - (age / lifetime)), maxA);
			colourer.color = color;
			
			if (age > lifetime) {
				NetworkServer.Destroy (gameObject);
			}
		}
	}
}