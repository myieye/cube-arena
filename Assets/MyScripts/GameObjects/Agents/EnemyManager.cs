using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.GameObjects.Agents {
	public class EnemyManager : MonoBehaviourSingleton<EnemyManager> {

		[SerializeField]
		private GameObject[] enemyPrefabs;
		
		private int enemyCount;

		private int MaxLevel { get { return (enemyPrefabs.Length * 2) - 1; } }

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
			var nextLevelI = Mathf.Min (enemy.level, MaxLevel);
			var newEnemy = enemyPrefabs[nextLevelI / 2];
			EnemySpawner.Instance.SpawnEnemy (newEnemy, nextLevelI % 2 != 0, enemy.level + 1);
		}

		private void SpawnEnemies () {
			while (enemyCount * Settings.Instance.PlayersPerEnemy < PlayerManager.Instance.NumberOfActivePlayers) {
				EnemySpawner.Instance.SpawnEnemy (enemyPrefabs[0], false, 1);
				enemyCount++;
			}
		}
	}
}