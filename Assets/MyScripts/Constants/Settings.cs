using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Constants {
        public class Settings : MonoBehaviour {
                public bool AREnabled;

                [Header("Rotation")]
                public float RotationTimeout;
                public float MinRotationVelocity;
                public float MaxRotationVelocity;
        }
}