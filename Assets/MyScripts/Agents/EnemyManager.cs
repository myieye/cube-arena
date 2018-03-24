using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Agents
{
	public class EnemyManager : MonoBehaviour {

		public EnemySpawner enemySpawner;
		public int playersPerEnemy;
		public GameObject[] enemyPrefabs;
		private int playerCount;
		private int enemyCount;

		private int MaxLevel { get { return enemyPrefabs.Length - 1; } }

		void Start () {
			Reset();
		}
		
		void Update () {
			
		}

		public void OnPlayerAdded() {
			playerCount++;
			if (enemyCount * playersPerEnemy < playerCount) {
				enemySpawner.SpawnEnemy(enemyPrefabs[0]);
				enemyCount++;
			}
		}

		public void OnEnemyKilled(Enemy enemy) {
			var newEnemy = enemyPrefabs[Mathf.Min(enemy.level, MaxLevel)];
			enemySpawner.SpawnEnemy(newEnemy);
		}

		public void Reset() {
			playerCount = 0;
			enemyCount = 0;
		}
	}
}