using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.GameObjects.Agents {
	public class EnemySpawner : MonoBehaviour {

		public Collider area;
		private ARManager arManager;
		private float radius;
		private List<GameObject> enemySpawnList = new List<GameObject> ();
		private List<OverlapManager> enemyOverlapDetecters = new List<OverlapManager> ();

		void Awake () {
			radius = area.bounds.center.x - area.bounds.min.x;
			var seed = System.Guid.NewGuid ().GetHashCode ();
			Random.InitState (seed);
		}

		void Start () {
			if (Settings.Instance.AREnabled) {
				arManager = FindObjectOfType<ARManager> ();
			}
		}

		void Update () {
			for (var i = 0; i < enemySpawnList.Count; i++) {
				var enemy = enemySpawnList[i];
				if (!enemyOverlapDetecters[i].HasOverlap ()) {
					EnableEnemy (enemy);
					if (Settings.Instance.AREnabled) {
						arManager.AddGameObjectToWorld (enemy);
					}
					NetworkServer.Spawn (enemy);
					i--;
				} else {
					enemy.transform.position = GetRandomPosition ();
				}
			}
		}

		public void SpawnEnemy (GameObject enemyPrefab) {
			DisableEnemy (Instantiate (enemyPrefab, GetRandomPosition (), Random.rotation));
		}

		public Vector3 GetRandomPosition () {
			Vector3 pos = Random.insideUnitCircle * radius;
			pos.z = pos.y;
			pos.y = 0;
			return pos;
		}

		private void DisableEnemy (GameObject enemy) {
			enemy.GetComponent<Enemy> ().enabled = false;
			var nav = enemy.GetComponent<RandomAgentNavigation> ();
			nav.enemyManager = this;
			nav.enabled = false;
			foreach (var c in enemy.GetComponentsInChildren<Collider> ())
				c.isTrigger = true;
			foreach (var r in enemy.GetComponentsInChildren<Renderer> ())
				r.enabled = false;
			enemySpawnList.Add (enemy);
			enemyOverlapDetecters.Add (enemy.GetComponent<OverlapManager> ());
		}

		private void EnableEnemy (GameObject enemy) {
			enemy.GetComponent<Enemy> ().enabled = true;
			enemy.GetComponent<RandomAgentNavigation> ().enabled = true;
			Destroy (enemy.GetComponent<OverlapManager> ());
			foreach (var c in enemy.GetComponentsInChildren<Collider> ())
				c.isTrigger = false;
			foreach (var r in enemy.GetComponentsInChildren<Renderer> ())
				r.enabled = true;
			enemySpawnList.Remove (enemy);
			enemyOverlapDetecters.Remove (enemy.GetComponent<OverlapManager> ());
		}
	}
}