using UnityEngine;

namespace CubeArena.Assets.MyScripts.Logging.DAL.Models {
    public class Selection : TimedMeasurement {

        [SerializeField]
        private byte dummyField;

        public override string ToString () {
            return string.Format (
                "Selection: [{0}]", base.ToString ());
        }
    }
}