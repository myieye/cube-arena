using CubeArena.Assets.MyScripts.Logging.DAL.SQLite;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Logging.DAL.Models {
    public class Selection : TimedMeasurement {

        [HideInInspector]
        public byte dummyField;

        public override string ToString () {
            return string.Format (
                "Selection: [{0}]", base.ToString ());
        }
    }
}