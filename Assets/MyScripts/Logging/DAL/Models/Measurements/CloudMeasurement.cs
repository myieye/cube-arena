using CubeArena.Assets.MyScripts.Logging.DAL.SQLite;

namespace CubeArena.Assets.MyScripts.Logging.DAL.Models {
    public class CloudMeasurement : Measurement {
        public float _overlapTime;
        public float _multipleOverlapTime;
        public int _numOverlaps;

        [NotNull]
        public float OverlapTime {
            get { return _overlapTime; }
            set { _overlapTime = value; }
        }

        [NotNull]
        public float MultipleOverlapTime {
            get { return _multipleOverlapTime; }
            set { _multipleOverlapTime = value; }
        }

        [NotNull]
        public int NumOverlaps {
            get { return _numOverlaps; }
            set { _numOverlaps = value; }
        }

        public override string ToString () {
            return string.Format (
                "CloudMeasurement: [{0} Overlap Time: {1}. Multiple Overlap Time: {2}. Num Overlaps: {3}.]",
                base.ToString (), OverlapTime, MultipleOverlapTime, NumOverlaps);
        }
    }
}