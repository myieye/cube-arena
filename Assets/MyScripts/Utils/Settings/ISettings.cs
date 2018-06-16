using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Settings;

namespace CubeArena.Assets.MyScripts.Utils.Settings {
	public interface ISettings {

		bool EnableUserStudySettings ();
        bool CheckUserStudySettings ();

		bool ForceUserStudySettings { get; }
		string ServerIp { get; set; }
        bool StartServerAutomatically { get; }
		bool StartClientAutomatically { get; }
		PlayerNumberMode PlayerNumberMode { get; }
		int NumberOfPlayers { get; }
		int PlayersPerEnemy { get; }
        bool SkipTestPhase { get; }
		float RoundLength { get; }
		float PracticeRoundLength { get; }
		float ShortPracticeRoundLength { get; }
		float PassToPlayerTime { get; }
		bool AREnabledInEditor { get; }
		bool AREnabled { get; }
		bool OptimizeDeviceRoundConfig { get; }
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
		bool DisableUIModeListOnClients { get; }
		bool ForceTestUIMode { get; }
		UIMode TestUIMode { get; }
		bool ForceDefaultUIMode { get; }
		UIMode DefaultHHDUIMode { get; }
		UIMode DefaultHMDUIMode { get; }
		UIMode DefaultUIMode { get; }
		bool LogDbVersion { get; }
		bool ResetDebugDbOnStart { get; }
		DatabaseVersion DefaultDatabaseVersion { get; }
        bool PrintDbTablesOnInit { get; }
		bool LogMeasurementsToConsole { get; }
		bool ServerOnlyMeasurementLogging { get; }
		int InteractionAreaGridSize { get; }
		float RotationTimeout { get; }
		float MinRotationVelocity { get; }
		float MaxRotationVelocity { get; }
		float AxisSensitivity { get; }
		//int[] AreaRadiuses { get; }
		//float AreaCenterPlayerStartPointOffset { get; }
	}
}