using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Fire;
using CubeArena.Assets.MyScripts.Logging;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Agents
{
	public class Enemy : NetworkBehaviour {
		
		public int level;
		private EnemyManager enemyManager;

		void Start() {
			enemyManager = FindObjectOfType<EnemyManager>();
		}

		void OnCollisionEnter(Collision col) {
			if (!isServer) return;

			if (IsDetchCollision(col)) {
				RpcDie(col.gameObject);
				enemyManager.OnEnemyKilled(this);
				Measure.MadeKill(col.gameObject, this);
			}
		}

		[ClientRpc]
		private void RpcDie(GameObject cube) {
			//cube.transform.Translate(Vector3.up);
			foreach (var collider in GetComponentsInChildren<Collider>()) {
				collider.enabled = false;
			}
			GetComponent<Animator>().enabled = false;
			Destroy(gameObject, 3);
		}

		private bool IsDetchCollision(Collision col) {
			return
				col.gameObject.CompareTag(Constants.Tags.Cube) &&
				col.gameObject.GetComponentInChildren<DynamicFireSource>().burning;
			//return col.gameObject.transform.position.y > transform.position.y;
		}
	}
}