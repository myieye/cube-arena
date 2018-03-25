namespace CubeArena.Assets.MyScripts.Data.Models {
    public class Selection : TimedMeasurement {
        
        public override string ToString() {
            return string.Format (
                "Selection: [{0}]", base.ToString ());
        }
    }
}