using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.UI.Mode;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Constants {
        public class Settings : MonoBehaviour {
                public bool AREnabled;
                public bool LogInteractionStateChanges;
                public bool DebugCursor;
                
                [Header ("UI Mode")]
                public bool UITestMode;
                public UIMode TestUI;
                
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

                void Start () {
                        Instance = this;
                }
        }
}