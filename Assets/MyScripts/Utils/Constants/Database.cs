using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Constants {
    public class Database {
        public const string CubeArenaMeasurementsDatabase = "CubeArenaMeasurements";
    }

    public enum DatabaseVersion { Debug, Release, Mock }

    public static class DatabaseVersionExtensions {
        public static Color GetColor (this DatabaseVersion dbVersion) {

            switch (dbVersion) {
                case DatabaseVersion.Release:
                    return Color.green;
                default:
                    return Color.red;
            }
        }
    }
}