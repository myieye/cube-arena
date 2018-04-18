using System;
using CubeArena.Assets.MyScripts.Logging.DAL.SQLite;

namespace CubeArena.Assets.MyScripts.Logging.DAL.Models {
    public class Move : TimedMeasurement {

        public float _distance;
        public float _cumulativeDistance;

        [NotNull]
        public float Distance {
            get { return _distance; }
            set { _distance = value; }
        }

        [NotNull]
        public float CumulativeDistance {
            get { return _cumulativeDistance; }
            set { _cumulativeDistance = value; }
        }

        public override string ToString () {
            return string.Format (
                "Move: [{0} Distance: {1}. Cumulative Distance: {2}.]",
                base.ToString (), Distance, CumulativeDistance);
        }
    }
}