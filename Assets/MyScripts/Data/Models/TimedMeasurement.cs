using CubeArena.Assets.MyScripts.Data.SQLite;

namespace CubeArena.Assets.MyScripts.Data.Models
{
    public abstract class TimedMeasurement : Measurement {

        public double _time;
        
        [NotNull]
        public double Time {
            get { return _time; }
            set { _time = value; }
        }
        
        public override string ToString() {
            return string.Format (
                "{0} Time: {1}.",
                base.ToString (), Time);
        }
    }
}