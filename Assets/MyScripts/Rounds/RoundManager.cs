using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Agents;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.Network;
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
		public float practiceRoundLength;
		public int numRounds;
		public bool InPracticeMode { get; private set; }
		private int currRound = 0;
		// Flag for reacting to Scene Loads triggered by RoundManager
		//	Currently RoundManager is the only class that causes Scene changes
		private bool startingRound = false;
		
		void Start () {
			InPracticeMode = false;
		}

		[Server]
		public bool OnSceneLoaded() {
			if (startingRound) {
				// Set flag to ignore future OnSceneLoaded events
				startingRound = false;
	
				if (InPracticeMode) {
					timeManager.StartRound(practiceRoundLength, this);
				} else {
					timeManager.StartRound(roundLength, this);
				}
				return true;
			}
			return false;
		}

		[Server]
		public void StartNewRound() {
			// Toggle practice mode
			if (InPracticeMode = !InPracticeMode) {
				// If entering practice mode, the next round is starting
				currRound++;
				// Generate new PlayerRoundIds
				PlayerManager.Instance.GeneratePlayerRoundIds(currRound);
			}
			enemyManager.Reset();
			// Set flag to react to next OnSceneLoaded event
			startingRound = true;
            // Reload the scene
            UnityEngine.Networking.NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
		}

		public void OnRoundOver() {
			Measure.FlushMeasurements();
			if (!InLastRound()) {
				StartNewRound();
			}
		}

		private bool InLastRound() {
			return currRound < numRounds || (currRound == numRounds && InPracticeMode);
		}
	}
}