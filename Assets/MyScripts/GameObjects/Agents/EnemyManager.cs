using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.GameObjects.Agents {
	public class EnemyManager : MonoBehaviourSingleton<EnemyManager> {

		[SerializeField]
		private int playersPerEnemy;
		[SerializeField]
		private GameObject[] enemyPrefabs;
		
		private int enemyCount;

		private int MaxLevel { get { return enemyPrefabs.Length - 1; } }

		public void InitEnemies () {
			enemyCount = 0;
			SpawnEnemies ();
		}

		public void ClearEnemies () {
			foreach (var enemy in FindObjectsOfType<Enemy> ()) {
				NetworkServer.Destroy (enemy.gameObject);
			}
			EnemySpawner.Instance.StopSpawning ();
			enemyCount = 0;
		}

		public void OnEnemyKilled (Enemy enemy) {
			var newEnemy = enemyPrefabs[Mathf.Min (enemy.level, MaxLevel)];
			EnemySpawner.Instance.SpawnEnemy (newEnemy);
		}

		private void SpawnEnemies () {
			while (enemyCount * playersPerEnemy < PlayerManager.Instance.NumPlayers) {
				EnemySpawner.Instance.SpawnEnemy (enemyPrefabs[0]);
				enemyCount++;
			}
		}
	}
}