namespace CubeArena.Assets.MyScripts.Logging.DAL.Models {
    public class Placement : Measurement {

        public int _placedOnPlayerId;

        public int PlacedOnPlayerId {
            get { return _placedOnPlayerId; }
            set { _placedOnPlayerId = value; }
        }

        public override string ToString () {
            return string.Format (
                "Placement: [{0} PlacedOnPlayerId: {1}.]",
                base.ToString (), PlacedOnPlayerId);
        }
    }
}