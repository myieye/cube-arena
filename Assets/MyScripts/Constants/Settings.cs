using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Constants {
        public class Settings : MonoBehaviour {
                public bool AREnabled;

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