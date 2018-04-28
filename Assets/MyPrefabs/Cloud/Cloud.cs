using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyPrefabs.Cloud {

	[RequireComponent(typeof(Renderer), typeof(ARObject))]
	public class Cloud : NetworkBehaviour {

		[SerializeField]
		private float lifetime;
		private DateTime createdAt;
		private Renderer rend;
		private float maxA;

		void Start () {
			rend = GetComponent<Renderer>();
			maxA = rend.material.color.a;

			// Server Code ------------------------
			if (!isServer) return;
			createdAt = DateTime.Now;
		}
		
		void Update () {
			var age = (DateTime.Now - createdAt).TotalMilliseconds;
			var color = rend.material.color;
			color.a = Mathf.Min((float) (1 - (age/lifetime)), maxA);
			rend.material.color = color;

			// Server Code ------------------------
			if (!isServer) return;
			if (age > lifetime) {
				NetworkServer.Destroy(gameObject);
			}
		}
	}
}