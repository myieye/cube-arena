using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Constants {
	public class Settings : MonoBehaviour {
		public bool AREnabled;
		public bool DebugCursor;
		public bool LogInteractionStateChanges;
		public bool LogCubeStateChanges;
        public bool LogDeviceRoundConfig;

		[Header ("Measurements")]
		public bool LogMeasurementsToConsole;
		public bool LogMeasurementsToDb;
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

#if UNITY_EDITOR || UNITY_STANDALONE
			AREnabled = false;
#endif
		}
	}
}