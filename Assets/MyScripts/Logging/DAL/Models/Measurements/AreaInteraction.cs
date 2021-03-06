using CubeArena.Assets.MyScripts.Logging.DAL.SQLite;

namespace CubeArena.Assets.MyScripts.Logging.DAL.Models {
    public class AreaInteraction : TimedMeasurement {

        public int _area;

        [NotNull]
        public int Area	
        {
            get { return _area;}
            set { _area = value;}
        }
        
        public override string ToString() {
            return string.Format (
                "Area Interaction: [{0} Area: {1}.]",
                base.ToString (), Area);
        }
    }
}