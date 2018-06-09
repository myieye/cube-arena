using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyPrefabs.Cloud;
using CubeArena.Assets.MyScripts.GameObjects.Agents;
using CubeArena.Assets.MyScripts.GameObjects.Spray;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.Logging.Survey;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace CubeArena.Assets.MyScripts.PlayConfig.Rounds {
	public class RoundManager : NetworkBehaviourSingleton, RoundOverListener, SurveyFinishedListener {

		public bool InPracticeMode { get; private set; }
		public bool InTestPhase { get; private set; }
		public int NumberOfRounds {
			get {
				return UIModeHelpers.TestUIModes.Count;
			}
		}
		private TimeManager timeManager;
		private int currRound;
		private List<List<DeviceConfig>> deviceRoundConfigs;
		private DeviceConfigurationGenerator configGenerator;
		private float RoundLength {
			get {
				if (InTestPhase) {
					return Settings.Instance.PracticeRoundLength;
				} else if (InPracticeMode) {
					return Settings.Instance.ShortPracticeRoundLength;
				} else {
					return Settings.Instance.RoundLength;
				}
			}
		}
		private int NumberOfPlayers {
			get {
				if (InTestPhase) {
					return PlayerManager.Instance.NumberOfPlayersForRound;
				} else {
					return PlayerManager.Instance.NumberOfActivePlayers;
				}
			}
		}

		void Start () {
			timeManager = FindObjectOfType<TimeManager> ();
			configGenerator = new DeviceConfigurationGenerator (DeviceManager.Instance);
			ResetRoundCounter ();
		}

		void OnEnable () {
			ResetRoundCounter ();
		}

		public void OnRoundOver (bool force = false) {
			if (!force && Settings.Instance.EndlessRounds) return;

			ResetGameObjects (0.5f);
			timeManager.RpcClear ();

			if (!InTestPhase && !InPracticeMode && currRound > 0) {
				UIModeManager.Instance<UIModeManager> ().DisablePlayerUIs (PlayerManager.Instance.Players);
				FindObjectOfType<Surveyer> ().DoSurvey (PlayerManager.Instance.Players, this);
			} else {
				TriggerNewRound ();
			}
		}

		public void OnSurveyFinished () {
			if (InLastRound ()) {
				ResetRoundCounter ();
				UIModeManager.Instance<UIModeManager> ().DisablePlayerUIs (PlayerManager.Instance.Players);
				PlayerManager.Instance.DestroyPlayers ();
			} else {
				TriggerNewRound ();
			}
		}

		private void TriggerNewRound () {
			if (InLastRound ()) {
				ResetRoundCounter ();
			}

			ResetGameObjects (0.5f);
			StartCoroutine (DelayUtil.Do (0.5f, StartNewRound));
		}

		private void StartNewRound () {
			IncrementModeAndRoundNumber ();

			if (InPracticeMode) {
				if (NeedsNewRoundConfig ()) {
					if (InFirstRound ()) {
						Settings.Instance.CheckUserStudySettings ();
						DeviceManager.Instance.SaveConnectedDevicesToDb ();
					}

					if (!configGenerator.TryGenerateDeviceRoundConfigs (NumberOfPlayers, out deviceRoundConfigs)) {
						DecrementModeAndRoundNumber ();
						return;
					}
					if (InFirstRound ()) {
						PlayerManager.Instance.GenerateNewPlayers ();
					}
				}

				// Generate new PlayerRoundIds
				var players = PlayerManager.Instance.ConfigurePlayersForRound (
					currRound, deviceRoundConfigs[currRound - 1]);
				UIModeManager.Instance<UIModeManager> ().SetPlayerUIModes (players);

				timeManager.StartRound (RoundLength, Settings.Instance.PassToPlayerTime, this, InPracticeMode);
				StartCoroutine (DelayUtil.Do (Settings.Instance.PassToPlayerTime, SpawnGameObjects));
			} else {
				timeManager.StartRound (RoundLength, 0, this, InPracticeMode);
				SpawnGameObjects ();
			}
		}

		private void IncrementModeAndRoundNumber () {
			if (currRound == NumberOfRounds && InTestPhase) {
				InTestPhase = false;
				InPracticeMode = true;
				currRound = 1;
			} else if (InTestPhase) {
				InPracticeMode = true;
				currRound++;
			} else {
				InPracticeMode = !InPracticeMode;
				if (InPracticeMode) {
					currRound++;
				}
			}
		}

		private void DecrementModeAndRoundNumber () {
			if (InTestPhase) {
				currRound--;
			} else {
				InPracticeMode = !InPracticeMode;
				if (!InPracticeMode) {
					currRound--;
				}
			}
		}

		private void SpawnGameObjects () {
			EnemyManager.Instance.InitEnemies ();
			PlayerManager.Instance.SpawnPlayers ();
		}

		private void ResetGameObjects (float time) {
			SprayManager.Instance<SprayManager> ().ResetSpray ();
			EnemyManager.Instance.ClearEnemies ();
			PlayerManager.Instance.ClearPlayers (time);
		}

		private void ResetRoundCounter () {
			InTestPhase = true;
			InPracticeMode = false;
			currRound = 0;
		}

		private bool InLastRound () {
			return currRound == NumberOfRounds && !InPracticeMode && !InTestPhase;
		}

		private bool InFirstRound () {
			return currRound == 1 && InTestPhase;
		}

		private bool NeedsNewRoundConfig () {
			return currRound == 1 && (InPracticeMode || InTestPhase);
		}
	}
}