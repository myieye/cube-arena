using System;
using CubeArena.Assets.MyScripts.Data.SQLite;

namespace CubeArena.Assets.MyScripts.Data.Models {
    public class Rotation : TimedMeasurement {

        public float _angle;
        public float _cumulativeAngle;

        [NotNull]
        public float Angle {
            get { return _angle; }
            set { _angle = value; }
        }
        [NotNull]
        public float CumulativeAngle
        {
            get { return _cumulativeAngle;}
            set { _cumulativeAngle = value;}
        }
        

        public override string ToString () {
            return string.Format (
                "Rotation: [{0} Angle: {1}. Cumulative Angle: {2}.]",
                base.ToString (), Angle, CumulativeAngle);
        }
    }
}