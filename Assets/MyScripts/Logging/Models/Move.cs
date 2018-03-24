using System;
using CubeArena.Assets.MyScripts.Logging.SQLite;

namespace CubeArena.Assets.MyScripts.Logging.Models {
    public class Move : TimedMeasurement {

        public float _distance;

        [NotNull]
        public float Distance {
            get { return _distance; }
            set { _distance = value; }
        }

        public override string ToString () {
            return string.Format (
                "Move: [{0} Distance: {1}.]",
                base.ToString (), Distance);
        }
    }
}