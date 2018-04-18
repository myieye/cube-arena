using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.GameObjects.Agents {
	public class EnemyManager : MonoBehaviour {

		public int playersPerEnemy;
		public GameObject[] enemyPrefabs;
		
		private EnemySpawner enemySpawner;
		private int enemyCount;
		private PlayerManager playerManager;

		private int MaxLevel { get { return enemyPrefabs.Length - 1; } }

		void Start () {
			enemySpawner = FindObjectOfType<EnemySpawner>();
			playerManager = FindObjectOfType<PlayerManager>();
		}

		public void InitEnemies () {
			enemyCount = 0;
			SpawnEnemies ();
		}

		public void OnEnemyKilled (Enemy enemy) {
			var newEnemy = enemyPrefabs[Mathf.Min (enemy.level, MaxLevel)];
			enemySpawner.SpawnEnemy (newEnemy);
		}

		private void SpawnEnemies () {
			while (enemyCount * playersPerEnemy < playerManager.NumPlayers) {
				enemySpawner.SpawnEnemy (enemyPrefabs[0]);
				enemyCount++;
			}
		}
	}
}