using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyPrefabs.Cloud;
using CubeArena.Assets.MyScripts.GameObjects.Agents;
using CubeArena.Assets.MyScripts.GameObjects.Spray;
using CubeArena.Assets.MyScripts.Logging.DAL;
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
		private bool gameStarted;
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
				if (gameStarted) {
					return PlayerManager.Instance.NumberOfActivePlayers;
				} else {
					return PlayerManager.Instance.NumberOfPlayersForRound;
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

		void OnDisable () {
			if (Settings.Instance.ForceUserStudySettings) {
				Debug.Log (string.Format ("Round on disable: {0}. PM: {1}. TP: {2}.",
					currRound, InPracticeMode, InTestPhase));
			}
		}

		public void OnRoundOver (bool force = false) {
			Debug.Log ("OnRoundOver: " + force);
			if (!force && Settings.Instance.EndlessRounds) return;
			Debug.Log ("yes");

			ResetGameObjects (0.5f);
			Debug.Log ("clear");
			timeManager.RpcClear (true);

			if (!InTestPhase && !InPracticeMode && currRound > 0) {
				UIModeManager.Instance<UIModeManager> ().DisablePlayerUIs (PlayerManager.Instance.Players);
				FindObjectOfType<Surveyer> ().DoSurvey (PlayerManager.Instance.Players, this, InLastRound ());
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

			if (!DoRoundSetup ()) {
				DecrementModeAndRoundNumber ();
				return;
			}

			float roundDelay = 0;
			if (IsTimeForNextUIMode ()) {
				roundDelay = Settings.Instance.PassToPlayerTime;
				PlayerManager.Instance.ConfigurePlayersForRound (currRound, deviceRoundConfigs[currRound - 1]);
			}

			StartCoroutine (DelayUtil.Do (roundDelay, SpawnGameObjects));
			timeManager.StartRound (RoundLength, roundDelay, this, InPracticeMode);
			UIModeManager.Instance<UIModeManager> ().SetPlayerUIModes (
				PlayerManager.Instance.Players, roundDelay + 0.2f, IsTimeForNextUIMode ());

			if (!gameStarted) {
				gameStarted = true;
				CustomNetworkDiscovery.Instance.StopBroadcasting ();
				if (Settings.Instance.ForceUserStudySettings) {
					Debug.LogFormat ("Starting game. Round: {0}. PM: {1}. TP: {2}.", currRound, InPracticeMode, InTestPhase);
				}
			} else {
				if (Settings.Instance.ForceUserStudySettings) {
					Debug.LogFormat ("Starting Round: {0}. PM: {1}. TP: {2}.", currRound, InPracticeMode, InTestPhase);
				}
			}
		}

		private bool DoRoundSetup () {
			if (InFirstRound ()) {
				if (!Settings.Instance.CheckUserStudySettings ()) {
					return false;
				}
				DeviceManager.Instance.SaveConnectedDevicesToDb ();
			}

			if (NeedsNewRoundConfig ()) {
				if (!InitDeviceRoundConfig ()) {
					return false;
				}
			}

			return true;
		}

		private bool InitDeviceRoundConfig () {
			switch (GameConfigManager.Instance.Mode) {

				case GameConfigMode.New:
					if (configGenerator.TryGenerateDeviceRoundConfigs (NumberOfPlayers, out deviceRoundConfigs)) {
						if (InFirstRound ()) {
							PlayerManager.Instance.GenerateNewPlayers ();
						}
						PlayerManager.Instance.GeneratePlayerRounds (deviceRoundConfigs);
						return true;
					}
					break;

				case GameConfigMode.Old:
					var gameConfig = GameConfigManager.Instance.GetSelectedGameConfig ();
					if (configGenerator.MatchDevicesToGameConfig (gameConfig, out deviceRoundConfigs)) {
						PlayerManager.Instance.InitPlayersWithPlayerRounds (gameConfig);

						currRound = GameConfigManager.Instance.GetStartingRound ();
						return true;
					}
					break;
			}

			return false;
		}

		private void IncrementModeAndRoundNumber () {
			if (BeforeFirstRound () && Settings.Instance.SkipTestPhase) {
				InTestPhase = false;
			}

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
			gameStarted = false;
		}

		private bool InLastRound () {
			return currRound == NumberOfRounds && !InPracticeMode && !InTestPhase;
		}

		private bool InFirstRound () {
			return currRound == 1 && (InTestPhase || (Settings.Instance.SkipTestPhase && InPracticeMode));
		}

		private bool BeforeFirstRound () {
			return currRound == 0;
		}

		private bool NeedsNewRoundConfig () {
			return currRound == 1 && (InPracticeMode || InTestPhase);
		}

		private bool IsTimeForNextUIMode () {
			return InPracticeMode;
		}
	}
}