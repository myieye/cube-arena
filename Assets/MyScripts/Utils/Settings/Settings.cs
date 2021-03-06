﻿using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Logging.DAL;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils.Attributes;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Settings {
	public class Settings : MonoBehaviour, ISettings {

		void Awake () {
			if (Instance != null) {
				Destroy (this);
				return;
			}

			Instance = this;

			CheckUserStudySettings ();

			//skipTestPhase = false;

#if UNITY_EDITOR
			arEnabled = AREnabledInEditor && WebCamTexture.devices.Length > 0;
#elif UNITY_STANDALONE
			arEnabled = false;
#else
			arEnabled = true;
#endif

#if !UNITY_EDITOR
			defaultDatabaseVersion = DatabaseVersion.Mock;
			//serverIp = "192.168.137.1";
#endif
		}

		public bool CheckUserStudySettings () {
			if (forceUserStudySettings) {
				return EnableUserStudySettings ();
			}
			return true;
		}

		public bool EnableUserStudySettings () {
			forceUserStudySettings = true;
			autoStartGame = false;
			endlessRounds = false;
			debugCursor = false;
			disableUIModeListOnClients = true;
			forceTestUIMode = false;
			forceDefaultUIMode = false;
			serverOnlyMeasurementLogging = false;

			if (DeviceTypeManager.DeviceType.IsServerDeviceType ()) {
				logDeviceConnections = true;
				resetDebugDbOnStart = false;
				overrideAvailableDevices = false;
#if UNITY_EDITOR || UNITY_STANDALONE
				if (!DatabaseManager.Instance) {
					Debug.LogError ("Database Manager not found");
					return false;
				} else if (DatabaseManager.Instance.SelectedDbVersion != DatabaseVersion.Release) {
					Debug.LogError ("Please select Release Database");
					return false;
				}
#endif
			} else {
				overrideAvailableDevices = true;
			}

#if UNITY_WSA && !UNITY_EDITOR
			var fpsText = GameObject.Find ("FpsText");
			if (fpsText) {
				fpsText.SetActive (false);
			}
			var fpsDisplay = GameObject.Find ("FPSDisplay");
			if (fpsDisplay) {
				fpsDisplay.SetActive (false);
			}
#endif
			return true;
		}

		public static ISettings Instance { get; set; }

		[Header ("User Study")]
		[SerializeField]
		private bool forceUserStudySettings;
		[SerializeField]
		private string serverIp;
		[SerializeField]
		private bool startServerAutomatically;
		[SerializeField]
		private bool startClientAutomatically;

		[Header ("Players")]
		[SerializeField]
		private PlayerNumberMode playerNumberMode;
		[SerializeField]
		[ConditionalField ("playerNumberMode", PlayerNumberMode.Custom)]
		private int numberOfPlayers;

		[Header ("Enemies")]
		[SerializeField]
		private int playersPerEnemy;

		[Header ("Rounds")]
		[SerializeField]
		private bool skipTestPhase;
		[SerializeField]
		private float roundLength;
		[SerializeField]
		private float practiceRoundLength;
		[SerializeField]
		private float shortPracticeRoundLength;
		[SerializeField]
		private float passToPlayerTime;

		[Header ("AR")]
		[SerializeField]
		private bool arEnabledInEditor;
		[SerializeField]
		private bool arEnabled;

		[Header ("Testing")]
		[SerializeField]
		private bool optimizeDeviceRoundConfig;
		[SerializeField]
		private bool overrideAvailableDevices;
		[SerializeField]
		private bool endlessRounds;

		[Header ("Debugging")]
		[SerializeField]
		private bool autoStartGame;
		[SerializeField]
		private bool debugCursor;
		[SerializeField]
		private bool logInteractionStateChanges;
		[SerializeField]
		private bool logCubeStateChanges;
		[SerializeField]
		private bool logDeviceRoundConfig;
		[SerializeField]
		private bool logUIMode;
		[SerializeField]
		private bool logDeviceInfo;
		[SerializeField]
		private bool logDeviceConnections;
		[Header ("UI Modes")]
		[SerializeField]
		private bool disableUIModeListOnClients;
		[SerializeField]
		private bool forceTestUIMode;
		[SerializeField]
		private UIMode testUIMode;
		[SerializeField]
		private bool forceDefaultUIMode;
		[SerializeField]
		private UIMode defaultHHDUIMode;
		[SerializeField]
		private UIMode defaultHMDUIMode;

		[Header ("Measurements")]
		[SerializeField]
		private bool printDbTablesOnInit;
		[SerializeField]
		private bool logMeasurementsToConsole;
		[SerializeField]
		private bool logDbVersion;
		[SerializeField]
		private bool resetDebugDbOnStart;
		[SerializeField]
		private DatabaseVersion defaultDatabaseVersion;
		[SerializeField]
		private bool serverOnlyMeasurementLogging;
		[SerializeField]
		private int interactionAreaGridSize;

		[Header ("Rotation")]
		[SerializeField]
		private float rotationTimeout;
		[SerializeField]
		private float minRotationVelocity;
		[SerializeField]
		private float maxRotationVelocity;
		[SerializeField]
		private float axisSensitivity;

		/*[SerializeField]
		private int[] areaRadiuses;
		[SerializeField]
		private float areaCenterPlayerStartPointOffset;*/

		public bool ForceUserStudySettings {
			get { return forceUserStudySettings; }
		}
		public string ServerIp {
			get { return serverIp; }
			set { serverIp = value; }
		}
		public bool StartServerAutomatically {
			get { return startServerAutomatically; }
		}
		public bool StartClientAutomatically {
			get { return startClientAutomatically; }
		}
		public PlayerNumberMode PlayerNumberMode {
			get { return playerNumberMode; }
		}
		public int NumberOfPlayers {
			get { return numberOfPlayers; }
		}
		public int PlayersPerEnemy {
			get { return playersPerEnemy; }
		}
		public bool SkipTestPhase {
			get { return skipTestPhase; }
		}
		public float RoundLength {
			get { return roundLength; }
		}
		public float PracticeRoundLength {
			get { return practiceRoundLength; }
		}
		public float ShortPracticeRoundLength {
			get { return shortPracticeRoundLength; }
		}
		public float PassToPlayerTime {
			get { return passToPlayerTime; }
		}
		public bool AREnabledInEditor {
			get { return arEnabledInEditor; }
		}
		public bool AREnabled {
			get { return arEnabled; }
		}
		public bool OptimizeDeviceRoundConfig {
			get { return optimizeDeviceRoundConfig; }
		}
		public bool OverrideAvailableDevices {
			get { return overrideAvailableDevices; }
		}
		public bool EndlessRounds {
			get { return endlessRounds; }
		}
		public bool AutoStartGame {
			get { return autoStartGame; }
		}
		public bool DebugCursor {
			get { return debugCursor; }
		}
		public bool LogInteractionStateChanges {
			get { return logInteractionStateChanges; }
		}
		public bool LogCubeStateChanges {
			get { return logCubeStateChanges; }
		}
		public bool LogDeviceRoundConfig {
			get { return logDeviceRoundConfig; }
		}
		public bool LogUIMode {
			get { return logUIMode; }
		}
		public bool LogDeviceInfo {
			get { return logDeviceInfo; }
		}
		public bool LogDeviceConnections {
			get { return logDeviceConnections; }
		}
		public bool DisableUIModeListOnClients {
			get { return disableUIModeListOnClients; }
		}
		public bool ForceTestUIMode {
			get { return forceTestUIMode; }
		}
		public UIMode TestUIMode {
			get { return testUIMode; }
		}
		public bool ForceDefaultUIMode {
			get { return forceDefaultUIMode; }
		}
		public UIMode DefaultHHDUIMode {
			get { return defaultHHDUIMode; }
		}
		public UIMode DefaultHMDUIMode {
			get { return defaultHMDUIMode; }
		}
		public UIMode DefaultUIMode {
			get {
				if (ForceTestUIMode) {
					return TestUIMode;
				} else {
					switch (DeviceTypeManager.DeviceType) {
						case DeviceTypeSpec.Android:
							return DefaultHHDUIMode;
						case DeviceTypeSpec.HoloLens:
							return DefaultHMDUIMode;
						default:
							return UIMode.Mouse;
					}
				}
			}
		}
		public bool LogDbVersion {
			get { return logDbVersion; }
		}
		public bool ResetDebugDbOnStart {
			get { return resetDebugDbOnStart; }
		}
		public DatabaseVersion DefaultDatabaseVersion {
			get { return defaultDatabaseVersion; }
		}
		public bool PrintDbTablesOnInit {
			get { return printDbTablesOnInit; }
		}
		public bool LogMeasurementsToConsole {
			get { return logMeasurementsToConsole; }
		}
		public bool ServerOnlyMeasurementLogging {
			get { return serverOnlyMeasurementLogging; }
		}
		public int InteractionAreaGridSize {
			get { return interactionAreaGridSize; }
		}
		public float RotationTimeout {
			get { return rotationTimeout; }
		}
		public float MinRotationVelocity {
			get { return minRotationVelocity; }
		}
		public float MaxRotationVelocity {
			get { return maxRotationVelocity; }
		}
		public float AxisSensitivity {
			get { return axisSensitivity; }
		}
		/*public int[] AreaRadiuses {
			get { return areaRadiuses; }
		}
		public float AreaCenterPlayerStartPointOffset {
			get { return areaCenterPlayerStartPointOffset; }
		}*/
	}
}