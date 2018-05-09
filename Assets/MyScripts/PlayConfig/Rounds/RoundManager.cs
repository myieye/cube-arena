using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyPrefabs.Cloud;
using CubeArena.Assets.MyScripts.GameObjects.Agents;
using CubeArena.Assets.MyScripts.GameObjects.Spray;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils;
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
		private int numRounds;
		private int currRound;
		private List<List<DeviceConfig>> deviceRoundConfigs;

		void Start () {
			Init ();
		}

		void OnEnable () {
			ResetRoundCounter ();
		}

		private void Init () {
			timeManager = FindObjectOfType<TimeManager> ();
			numRounds = UIModeHelpers.UIModes.Count;
			ResetRoundCounter ();
		}

		public void TriggerNewRound () {
			if (InLastRound ()) {
				ResetRoundCounter ();
				PlayerManager.Instance.DestroyPlayers ();
			}

			if (!DeviceManager.Instance<DeviceManager> ()
				.EnoughDevicesAvailable (PlayerManager.Instance.NumPlayers)) {
				Debug.LogError ("Not enough devices!");
				if (!Settings.Instance.OverrideAvailableDevices) {
					return;
				}
			}

			StartNewRound ();
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
			UpdateModeAndRoundNumber ();

			if (!InFirstRound ()) {
				ResetGameObjects ();
			}

			practiceModeIndicator.SetActive (InPracticeMode);
			
			if (InPracticeMode) {
				if (InFirstRound ()) {
					var numPlayers = PlayerManager.Instance.GenerateNewPlayers ();
					deviceRoundConfigs = DeviceManager.Instance<DeviceManager> ()
						.GenerateDeviceRoundConfigs (numPlayers);
				}

				// Generate new PlayerRoundIds
				var players = PlayerManager.Instance.ConfigurePlayersForRound (
					currRound, deviceRoundConfigs[currRound - 1]);
				UIModeManager.Instance<UIModeManager> ().SetPlayerUIModes (players);

				timeManager.StartRound (practiceRoundLength, Settings.Instance.PassToPlayerTime, this);
				StartCoroutine (DelayUtil.Do (Settings.Instance.PassToPlayerTime, SpawnGameObjects));
			} else {
				timeManager.StartRound (roundLength, 0, this);
				SpawnGameObjects ();
			}
		}

		private void UpdateModeAndRoundNumber () {
			// Toggle practice mode
			InPracticeMode = !InPracticeMode;
			// If entering practice mode, a new round is starting
			if (InPracticeMode) {
				currRound++;
			}
		}

		private void SpawnGameObjects () {
			EnemyManager.Instance.InitEnemies ();
			PlayerManager.Instance.SpawnPlayers ();
		}

		private void ResetGameObjects () {
			EnemyManager.Instance.ClearEnemies ();
			PlayerManager.Instance.ResetPlayers ();
			SprayManager.Instance.ResetSpray ();
		}

		private void ResetRoundCounter () {
			InPracticeMode = false;
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