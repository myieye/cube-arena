using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Constants {
	public class Settings : MonoBehaviour {
		[Header ("Device Management")]
		public float PassToPlayerTime;
		[Header ("AR")]
		public bool AREnabled;
		[Header ("Testing")]
		public bool OverrideAvailableDevices;
		public bool EndlessRounds;

		[Header ("Debugging")]
		public bool AutoStartGame;
		public bool DebugCursor;
		public bool LogInteractionStateChanges;
		public bool LogCubeStateChanges;
		public bool LogDeviceRoundConfig;
		public bool LogUIMode;
		public bool LogDeviceInfo;
		public bool LogDeviceConnections;
		[Header ("UI Modes")]
		public bool ForceTestUIMode;
		public UIMode TestUIMode;
		public bool ForceDefaultUIMode;
		public UIMode DefaultHHDUIMode;
		public UIMode DefaultHMDUIMode;
		public UIMode DefaultUIMode {
			get {
				if (ForceTestUIMode) {
					return TestUIMode;
				} else if (SystemInfo.deviceType == DeviceType.Handheld) {
					return DefaultHHDUIMode;
				} else {
					return DefaultHMDUIMode;
				}
			}
		}

		[Header ("Measurements")]
		public bool DbActive;
		public bool ResetDbOnStart;
		public bool LogMeasurementsToConsole;
		public bool ServerOnlyMeasurementLogging;

		[Header ("Rotation")]
		public float RotationTimeout;
		public float MinRotationVelocity;
		public float MaxRotationVelocity;
		public float AxisSensitivity;
		public int[] AreaRadiuses;
		public float AreaCenterPlayerStartPointOffset;

		public static Settings Instance { get; private set; }

		void Awake () {
			if (Instance != null) {
				Destroy (this);
				return;
			}

			Instance = this;

#if UNITY_STANDALONE
			AREnabled = false;
#elif !UNITY_EDITOR
			DbActive = false;
#endif
		}
	}
}