using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils.Constants;

namespace CubeArena.Assets.Tests.MyScripts.Utils.Constants {
    public class MockSettings : ISettings {

        public float PassToPlayerTime { get; set; }
        public bool AREnabledInEditor { get; set; }
        public bool AREnabled { get; set; }
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
        public bool DbActive { get; set; }
        public bool ResetDbOnStart { get; set; }
        public bool LogMeasurementsToConsole { get; set; }
        public bool ServerOnlyMeasurementLogging { get; set; }
        public float RotationTimeout { get; set; }
        public float MinRotationVelocity { get; set; }
        public float MaxRotationVelocity { get; set; }
        public float AxisSensitivity { get; set; }
        public int[] AreaRadiuses { get; set; }
        public float AreaCenterPlayerStartPointOffset { get; set; }
    }
}