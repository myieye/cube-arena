using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.Agents;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace CubeArena.Assets.MyScripts.PlayConfig.Rounds {
	public class RoundManager : NetworkBehaviour, RoundOverListener {

		public float roundLength;
		public float practiceRoundLength;
		public int numRounds;
		public bool InPracticeMode { get; private set; }
		public GameObject practiceModeIndicator;

		private TimeManager timeManager;
		private EnemyManager enemyManager;
		private int currRound;
		// Flag for reacting to Scene Loads triggered by RoundManager
		//	Currently RoundManager is the only class that causes Scene changes
		private bool startingRound = false;
		private List<List<DeviceConfig>> deviceRoundConfigs;

		void Start () {
			if (isServer) {
				SceneManager.sceneLoaded += OnSceneLoaded;
				timeManager = FindObjectOfType<TimeManager> ();
				enemyManager = FindObjectOfType<EnemyManager> ();
			}
		}

		public override void OnStartClient () {
			base.OnStartClient ();
			InPracticeMode = false;
			currRound = 0;
			startingRound = false;
		}

		[Server]
		public void OnSceneLoaded (Scene current, LoadSceneMode mode) {
			if (startingRound) {
				// Set flag to ignore future OnSceneLoaded events
				startingRound = false;
				timeManager.StartRound (InPracticeMode ? practiceRoundLength : roundLength, this);
				practiceModeIndicator.SetActive (InPracticeMode);
				enemyManager.InitEnemies ();
				PlayerManager.Instance.SpawnPlayers();
			}
		}

		[Server]
		public void StartNewRound () {
			// Set flag to react to next OnSceneLoaded event
			startingRound = true;
			// Toggle practice mode
			if (InPracticeMode = !InPracticeMode) {
				// If entering practice mode, the next round is starting
				currRound++;
				
				if (InFirstRound ()) {
					var numPlayers = PlayerManager.Instance.GenerateNewPlayers();
					deviceRoundConfigs = DeviceManager.Instance<DeviceManager> ()
						.GenerateDeviceRoundConfigs (numPlayers);
				}

				// Generate new PlayerRoundIds
				var players = PlayerManager.Instance.ConfigurePlayersForRound(
					currRound, deviceRoundConfigs[currRound - 1]);
				UIModeManager.Instance.SetPlayerUIModes(players);
			}
			// Reload the scene
			UnityEngine.Networking.NetworkManager.singleton.ServerChangeScene (SceneManager.GetActiveScene ().name);
		}

		public void OnRoundOver () {
			Measure.Instance.FlushMeasurements ();
			if (!InLastRound ()) {
				StartNewRound ();
			} else {
				currRound = 0;
			}
		}

		private bool InLastRound () {
			return currRound == numRounds && !InPracticeMode;
		}

		private bool InFirstRound () {
			return currRound == 1 && InPracticeMode;
		}
	}
}