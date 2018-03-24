using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Agents;
using CubeArena.Assets.MyScripts.UI.Mode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace CubeArena.Assets.MyScripts.Rounds
{
	public class RoundManager : NetworkBehaviour, RoundOverListener {

		public TimeManager timeManager;
		public EnemyManager enemyManager;
		public float roundLength;
		public int numRounds;
		private int currRound = 0;
		private bool startingRound = false;
		
		void Start () {
			
		}

		public bool OnSceneLoaded() {
			if (startingRound) {
				Debug.Log("startingRound");
				timeManager.StartRound(roundLength, this);
				startingRound = false;
				return true;
			}
			return false;
		}

		public void StartNewRound() {
			currRound++;
			startingRound = true;
			enemyManager.Reset();
			NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
		}

		public void OnRoundOver() {
			if (currRound < numRounds) {
				StartNewRound();
			}
		}
	}
}