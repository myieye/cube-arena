using CubeArena.Assets.MyScripts.PlayConfig.UIModes;

namespace CubeArena.Assets.MyScripts.Utils.Constants {
	public interface ISettings {
		float PassToPlayerTime  { get; }
		bool AREnabledInEditor { get; }
		bool AREnabled { get; }
		bool OverrideAvailableDevices { get; }
		bool EndlessRounds { get; }
		bool AutoStartGame { get; }
		bool DebugCursor { get; }
		bool LogInteractionStateChanges { get; }
		bool LogCubeStateChanges { get; }
		bool LogDeviceRoundConfig { get; }
		bool LogUIMode { get; }
		bool LogDeviceInfo { get; }
		bool LogDeviceConnections { get; }
		bool ForceTestUIMode { get; }
		UIMode TestUIMode { get; }
		bool ForceDefaultUIMode { get; }
		UIMode DefaultHHDUIMode { get; }
		UIMode DefaultHMDUIMode { get; }
		UIMode DefaultUIMode { get; }
		bool DbActive { get; }
		bool ResetDbOnStart { get; }
		bool LogMeasurementsToConsole { get; }
		bool ServerOnlyMeasurementLogging { get; }
		float RotationTimeout { get; }
		float MinRotationVelocity { get; }
		float MaxRotationVelocity { get; }
		float AxisSensitivity { get; }
		int[] AreaRadiuses { get; }
		float AreaCenterPlayerStartPointOffset { get; }
	}
}