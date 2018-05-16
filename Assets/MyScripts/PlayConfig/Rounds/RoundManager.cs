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
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace CubeArena.Assets.MyScripts.PlayConfig.Rounds {
	public class RoundManager : NetworkBehaviour, RoundOverListener {

		public bool InPracticeMode { get; private set; }
		public GameObject practiceModeIndicator;

		private TimeManager timeManager;
		private int numRounds;
		private int currRound;
		private List<List<DeviceConfig>> deviceRoundConfigs;
		private DeviceConfigurationGenerator configGenerator;

		void Start () {
			Init ();
			configGenerator = new DeviceConfigurationGenerator (DeviceManager.Instance);
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
			}

			ResetGameObjects ();
			StartNewRound ();
		}

		public void OnRoundOver () {
			if (Settings.Instance.EndlessRounds) return;

			//Measure.Instance.FlushMeasurements ();
			if (!InLastRound ()) {
				TriggerNewRound ();
			} else {
				UIModeManager.Instance<UIModeManager> ().DisablePlayerUIs (PlayerManager.Instance.Players);
				currRound = 0;
				ResetGameObjects ();
				ResetRoundCounter ();
			}
		}

		private void StartNewRound () {
			IncrementModeAndRoundNumber ();

			if (InPracticeMode) {
				if (InFirstRound ()) {
					if (!configGenerator.TryGenerateDeviceRoundConfigs (
							PlayerManager.Instance.NumberOfPlayers, out deviceRoundConfigs)) {
						DecrementModeAndRoundNumber ();
						return;
					}
					PlayerManager.Instance.GenerateNewPlayers ();
				}

				// Generate new PlayerRoundIds
				var players = PlayerManager.Instance.ConfigurePlayersForRound (
					currRound, deviceRoundConfigs[currRound - 1]);
				UIModeManager.Instance<UIModeManager> ().SetPlayerUIModes (players);

				timeManager.StartRound (Settings.Instance.PracticeRoundLength, Settings.Instance.PassToPlayerTime, this);
				StartCoroutine (DelayUtil.Do (Settings.Instance.PassToPlayerTime, SpawnGameObjects));
			} else {
				timeManager.StartRound (Settings.Instance.RoundLength, 0, this);
				SpawnGameObjects ();
			}

			practiceModeIndicator.SetActive (InPracticeMode);
		}

		private void IncrementModeAndRoundNumber () {
			InPracticeMode = !InPracticeMode;
			if (InPracticeMode) {
				currRound++;
			}
		}

		private void DecrementModeAndRoundNumber () {
			InPracticeMode = !InPracticeMode;
			if (!InPracticeMode) {
				currRound--;
			}
		}

		private void SpawnGameObjects () {
			EnemyManager.Instance.InitEnemies ();
			PlayerManager.Instance.SpawnPlayers ();
		}

		private void ResetGameObjects () {
			EnemyManager.Instance.ClearEnemies ();
			PlayerManager.Instance.ClearPlayers ();
			SprayManager.Instance<SprayManager> ().ResetSpray ();
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