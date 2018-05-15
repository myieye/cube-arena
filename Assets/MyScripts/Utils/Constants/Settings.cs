using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Constants {
	public class Settings : MonoBehaviour, ISettings {

		void Awake () {
			if (Instance != null) {
				Destroy (this);
				return;
			}

			Instance = this;

#if UNITY_EDITOR
			arEnabled = AREnabledInEditor && WebCamTexture.devices.Length > 0;
#elif UNITY_STANDALONE
			arEnabled = false;
#else
			arEnabled = true;
#endif

#if !UNITY_EDITOR
			dbActive = false;
#endif
		}

		public static ISettings Instance { get; set; }

		[Header ("Device Management")]
		[SerializeField]
		private float passToPlayerTime;

		[Header ("AR")]
		[SerializeField]
		private bool arEnabledInEditor;
		[SerializeField]
		private bool arEnabled;

		[Header ("Testing")]
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
		private bool dbActive;
		[SerializeField]
		private bool resetDbOnStart;
		[SerializeField]
		private bool logMeasurementsToConsole;
		[SerializeField]
		private bool serverOnlyMeasurementLogging;

		[Header ("Rotation")]
		[SerializeField]
		private float rotationTimeout;
		[SerializeField]
		private float minRotationVelocity;
		[SerializeField]
		private float maxRotationVelocity;
		[SerializeField]
		private float axisSensitivity;
		[SerializeField]
		private int[] areaRadiuses;
		[SerializeField]
		private float areaCenterPlayerStartPointOffset;

		public float PassToPlayerTime {
			get { return passToPlayerTime; }
		}
		public bool AREnabledInEditor {
			get { return arEnabledInEditor; }
		}
		public bool AREnabled {
			get { return arEnabled; }
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

		public bool DbActive {
			get { return dbActive; }
		}

		public bool ResetDbOnStart {
			get { return resetDbOnStart; }
		}

		public bool LogMeasurementsToConsole {
			get { return logMeasurementsToConsole; }
		}

		public bool ServerOnlyMeasurementLogging {
			get { return serverOnlyMeasurementLogging; }
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

		public int[] AreaRadiuses {
			get { return areaRadiuses; }
		}

		public float AreaCenterPlayerStartPointOffset {
			get { return areaCenterPlayerStartPointOffset; }
		}
	}
}