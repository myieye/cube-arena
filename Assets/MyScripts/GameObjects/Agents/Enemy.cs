using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.GameObjects.Fire;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.GameObjects.Agents {
	
	public class Enemy : NetworkBehaviour {

		public int level;
		//private float killHeight;

		void Start () {
			//killHeight = transform.localScale.y * GetComponent<BoxCollider> ().bounds.min.y;
		}

		void OnCollisionEnter (Collision col) {
			if (!isServer) return;

			if (IsDetchCollision (col)) {
				RpcDie (col.gameObject);
				EnemyManager.Instance.OnEnemyKilled (this);
				Measure.Instance.MadeKill (col.gameObject, this);
			}
		}

		[ClientRpc]
		private void RpcDie (GameObject cube) {
			//cube.transform.Translate(Vector3.up);
			foreach (var collider in GetComponentsInChildren<Collider> ()) {
				collider.enabled = false;
			}
			GetComponent<Animator> ().enabled = false;
			Destroy (gameObject, 3);
		}

		private bool IsDetchCollision (Collision col) {
			return col.gameObject.CompareTag (Tags.Cube) &&
				col.gameObject.GetComponentInChildren<DynamicFireSource> ().burning
			/* &&
							col.gameObject.transform.position.y > killHeight*/
			;
		}
	}
}