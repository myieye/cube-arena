using System;
using CubeArena.Assets.MyScripts.Logging.SQLite;

namespace CubeArena.Assets.MyScripts.Logging.Models {
    public class Rotation : TimedMeasurement {

        public float _angle;

        [NotNull]
        public float Angle {
            get { return _angle; }
            set { _angle = value; }
        }

        public override string ToString () {
            return string.Format (
                "Rotation: [{0} Angle: {1}.]",
                base.ToString (), Angle);
        }
    }
}