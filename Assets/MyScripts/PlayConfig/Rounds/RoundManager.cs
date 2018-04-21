using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.Agents;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace CubeArena.Assets.MyScripts.PlayConfig.Rounds {
	public class RoundManager : NetworkBehaviour, RoundOverListener {

		public float roundLength;
		public float practiceRoundLength;
		public bool InPracticeMode { get; private set; }
		public GameObject practiceModeIndicator;

		private TimeManager timeManager;
		private EnemyManager enemyManager;
		private int numRounds;
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
			numRounds = Enum.GetValues (typeof (UIMode)).Length;
		}

		[Server]
		public void OnSceneLoaded (Scene current, LoadSceneMode mode) {
			if (startingRound) {
				// Set flag to ignore future OnSceneLoaded events
				startingRound = false;
				StartNewRound ();
			}
		}

		[Server]
		public void TriggerNewRound () {
			if (InLastRound()) {
				Reset();
			}

			if (!DeviceManager.Instance<DeviceManager> ()
				.EnoughDevicesAvailable (PlayerManager.Instance.NumPlayers)) {
				Debug.LogError ("Not enough devices!");
				return;
			}

			// Set flag to react to next OnSceneLoaded event
			startingRound = true;
			// Reload the scene
			UnityEngine.Networking.NetworkManager.singleton.ServerChangeScene (SceneManager.GetActiveScene ().name);
		}

		public void OnRoundOver () {
			if (Settings.Instance.EndlessRounds) return;

			Measure.Instance.FlushMeasurements ();
			if (!InLastRound ()) {
				TriggerNewRound ();
			} else {
				currRound = 0;
			}
		}

		private void StartNewRound () {
			// Toggle practice mode
			if (InPracticeMode = !InPracticeMode) {
				// If entering practice mode, the next round is starting
				currRound++;

				if (InFirstRound ()) {
					var numPlayers = PlayerManager.Instance.GenerateNewPlayers ();
					deviceRoundConfigs = DeviceManager.Instance<DeviceManager> ()
						.GenerateDeviceRoundConfigs (numPlayers);
				}

				// Generate new PlayerRoundIds
				var players = PlayerManager.Instance.ConfigurePlayersForRound (
					currRound, deviceRoundConfigs[currRound - 1]);
				UIModeManager.Instance<UIModeManager> ().SetPlayerUIModes (players);

			}

			if (InPracticeMode) {
				timeManager.StartRound (practiceRoundLength, Settings.Instance.PassToPlayerTime, this);
			} else {
				timeManager.StartRound (roundLength, 0, this);
			}
			
			practiceModeIndicator.SetActive (InPracticeMode);
			enemyManager.InitEnemies ();
			PlayerManager.Instance.SpawnPlayers ();
		}

		private void Reset() {
			InPracticeMode = false;
			startingRound = false;
			currRound = 0;
		}

		private bool InLastRound () {
			return currRound == numRounds && !InPracticeMode;
		}

		private bool InFirstRound () {
			return currRound == 1 && InPracticeMode;
		}
	}
}