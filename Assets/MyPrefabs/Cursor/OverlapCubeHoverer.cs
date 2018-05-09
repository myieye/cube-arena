using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.Utils;

namespace CubeArena.Assets.MyPrefabs.Cursor
{
	public class OverlapCubeHoverer : NetworkBehaviour {

		public OverlapManager overlapManager;
		private InteractionStateManager stateManager;

		void Start () {
			stateManager = GetComponent<InteractionStateManager>();
		}
		
		protected void OnTriggerEnter(Collider col) {
			overlapManager.OnTriggerEnter(col);
		}
		
		protected void OnTriggerExit(Collider col) {
			overlapManager.OnTriggerExit(col);
			if (overlapManager.HasMatchingTag(col.gameObject)) {
				if (stateManager.IsHovered(col.gameObject)) {
					stateManager.EndHover();
				}
			}
		}

		void Update () {
			if (!isLocalPlayer) return;
			if (stateManager.IsSpraying()) return;
			
			GameObject cube;
			if (overlapManager.GetLocalClosest(out cube)) {
				stateManager.StartHover(cube);
			}
		}
	}
}