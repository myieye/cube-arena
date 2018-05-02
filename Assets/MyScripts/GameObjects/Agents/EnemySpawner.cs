using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace CubeArena.Assets.MyScripts.GameObjects.Agents {
	public class EnemySpawner : MonoBehaviourSingleton<EnemySpawner> {

		private List<GameObject> enemySpawnList = new List<GameObject> ();
		private List<OverlapManager> enemyOverlapDetecters = new List<OverlapManager> ();
		private object enemyLock = new object ();

		public override void Awake () {
			base.Awake ();
			var seed = System.Guid.NewGuid ().GetHashCode ();
			Random.InitState (seed);
		}

		void Update () {
			lock (enemyLock) {
				for (var i = 0; i < enemySpawnList.Count; i++) {
					var enemy = enemySpawnList[i];

					if (!enemy) {
						enemySpawnList.Remove (enemy);
						enemyOverlapDetecters.Remove (enemyOverlapDetecters[i]);
						i--;
						continue;
					}

					if (!enemyOverlapDetecters[i].HasOverlap ()) {
						EnableEnemy (enemy);
						NetworkServer.Spawn (enemy);
						i--;
					} else {
						enemy.transform.position = GetRandomPosition ();
					}
				}
			}
		}

		public void StopSpawning () {
			lock (enemyLock) {
				enemySpawnList.Clear ();
				enemyOverlapDetecters.Clear ();
			}
		}

		public void SpawnEnemy (GameObject enemyPrefab) {
			DisableEnemy (Instantiate (enemyPrefab, GetRandomPosition (), Random.rotation));
		}

		public Vector3 GetRandomPosition () {
			Vector3 pos = Random.insideUnitCircle * TransformUtil.LocalRadius;
			pos.z = pos.y;
			pos.y = 0;
			return pos;
		}

		private void DisableEnemy (GameObject enemy) {
			enemy.GetComponent<ARObject> ().enabled = false;
			enemy.GetComponent<Enemy> ().enabled = false;
			var nav = enemy.GetComponent<RandomAgentNavigation> ();
			nav.enemySpawner = this;
			nav.enabled = false;
			foreach (var c in enemy.GetComponentsInChildren<Collider> ())
				c.isTrigger = true;
			foreach (var r in enemy.GetComponentsInChildren<Renderer> ())
				r.enabled = false;
			enemySpawnList.Add (enemy);
			enemyOverlapDetecters.Add (enemy.GetComponent<OverlapManager> ());
		}

		private void EnableEnemy (GameObject enemy) {
			enemy.GetComponent<ARObject> ().enabled = true;
			enemy.GetComponent<Enemy> ().enabled = true;
			enemy.GetComponent<RandomAgentNavigation> ().enabled = true;
			foreach (var c in enemy.GetComponentsInChildren<Collider> ())
				c.isTrigger = false;
			foreach (var r in enemy.GetComponentsInChildren<Renderer> ())
				r.enabled = true;
			enemySpawnList.Remove (enemy);
			enemyOverlapDetecters.Remove (enemy.GetComponent<OverlapManager> ());
		}
	}
}