using CubeArena.Assets.MyScripts.Logging.DAL.SQLite;

namespace CubeArena.Assets.MyScripts.Logging.DAL.Models {
    public class TimedMeasurement : Measurement {

        public float _time;

        [NotNull]
        public float Time {
            get { return _time; }
            set { _time = value; }
        }

        public override string ToString () {
            return string.Format (
                "{0} Time: {1}.",
                base.ToString (), Time);
        }
    }
}