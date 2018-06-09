using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Settings;

namespace CubeArena.Assets.Tests.MyScripts.Utils.Constants {
    public class MockSettings : ISettings {

        public void EnableUserStudySettings () {

        }

        public void CheckUserStudySettings () {
            
        }

        public bool ForceUserStudySettings { get; set; }
        public string ServerIp { get; set; }
        public PlayerNumberMode PlayerNumberMode { get; set; }
        public int NumberOfPlayers { get; set; }
        public int PlayersPerEnemy { get; set; }
        public float RoundLength { get; set; }
        public float PracticeRoundLength { get; set; }
        public float ShortPracticeRoundLength { get; set; }
        public float PassToPlayerTime { get; set; }
        public bool AREnabledInEditor { get; set; }
        public bool AREnabled { get; set; }
        public bool OptimizeDeviceRoundConfig { get; set; }
        public bool OverrideAvailableDevices { get; set; }
        public bool EndlessRounds { get; set; }
        public bool AutoStartGame { get; set; }
        public bool DebugCursor { get; set; }
        public bool LogInteractionStateChanges { get; set; }
        public bool LogCubeStateChanges { get; set; }
        public bool LogDeviceRoundConfig { get; set; }
        public bool LogUIMode { get; set; }
        public bool LogDeviceInfo { get; set; }
        public bool LogDeviceConnections { get; set; }
        public bool DisableUIModeListOnClients { get; set; }
        public bool ForceTestUIMode { get; set; }
        public UIMode TestUIMode { get; set; }
        public bool ForceDefaultUIMode { get; set; }
        public UIMode DefaultHHDUIMode { get; set; }
        public UIMode DefaultHMDUIMode { get; set; }
        public UIMode defaultUIMode;
        public UIMode DefaultUIMode {
            get {
                if (ForceTestUIMode) {
                    return TestUIMode;
                } else {
                    return defaultUIMode;
                }
            }
        }
        public bool LogDbVersion { get; set; }
        public bool ResetDebugDbOnStart { get; set; }
        public DatabaseVersion DefaultDatabaseVersion { get; set; }
        public bool LogMeasurementsToConsole { get; set; }
        public bool ServerOnlyMeasurementLogging { get; set; }
        public int InteractionAreaGridSize { get; set; }
        public float RotationTimeout { get; set; }
        public float MinRotationVelocity { get; set; }
        public float MaxRotationVelocity { get; set; }
        public float AxisSensitivity { get; set; }

        //public int[] AreaRadiuses { get; set; }
        //public float AreaCenterPlayerStartPointOffset { get; set; }
    }
}