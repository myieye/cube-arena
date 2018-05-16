using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;
using UnityEngine.Assertions;

namespace CubeArena.Assets.MyScripts.PlayConfig.Devices {
    public class DeviceConfigurationGenerator {

        private IDeviceManager deviceManager;

        public DeviceConfigurationGenerator (IDeviceManager deviceManager) {
            this.deviceManager = deviceManager;
        }

        public bool TryGenerateDeviceRoundConfigs (int numPlayers, out List<List<DeviceConfig>> config) {
            if (!CheckEnoughDevicesAvailable (numPlayers)) {
                config = null;
                return false;
            }

            var numRounds = UIModeHelpers.TestUIModes.Count;
            config = new List<List<DeviceConfig>> ();

            for (var i = 0; i < numRounds; i++) {
                config.Add (new List<DeviceConfig> ());
            }

            // Generate random valid device configuration
            var success = AddNextDeviceRecursive (config, 0, 0, numPlayers);

            if (success) {
                // Assign random UI modes
                AssignUIModesToDeviceRoundConfig (config, numPlayers);

                if (Settings.Instance.LogDeviceRoundConfig) {
                    PrintDeviceRoundConfig (config);
                }
            }

            return success;
        }

        private void AssignUIModesToDeviceRoundConfig (List<List<DeviceConfig>> config, int numPlayers) {
            for (int p = 0; p < numPlayers; p++) {
                var mixedUiModes = new List<UIMode> (UIModeHelpers.TestUIModes).Shuffle ();
                for (int r = 0; r < config.Count; r++) {
                    var deviceConfig = config[r][p];
                    var deviceType = deviceConfig.Device.Type;

                    UIMode deviceTypeUiMode;
                    var foundUiMode = mixedUiModes.RemoveFirst (out deviceTypeUiMode, uiMode => uiMode.IsForDeviceType (deviceType));
                    if (!foundUiMode && Settings.Instance.OverrideAvailableDevices) {
                        deviceTypeUiMode = UIModeHelpers.UIModeOrFirstCompatible (deviceTypeUiMode);
                        mixedUiModes.RemoveAt (0);
                    } else if (!foundUiMode) {
                        throw new ArgumentException ("No remainging UIMode could be found for device type: " + deviceType);
                    }

                    deviceConfig.UIMode = deviceTypeUiMode;
                }
                Assert.IsTrue (mixedUiModes.Count == 0);
            }
        }

        private bool CheckEnoughDevicesAvailable (int numPlayers) {
            if (!deviceManager.EnoughDevicesAvailable (numPlayers)) {
                Debug.LogError ("Not enough devices!");
                if (!Settings.Instance.OverrideAvailableDevices) {
                    return false;
                }
            }
            return true;
        }

        private bool AddNextDeviceRecursive (List<List<DeviceConfig>> config, int roundI, int playerI, int numPlayers) {
            int numDeviceTypes = DeviceTypeSpecHelpers.TestDeviceTypes.Count;
            var mixedDevicesTypes = new List<DeviceTypeSpec> (DeviceTypeSpecHelpers.TestDeviceTypes).Shuffle ();

            for (var i = 0; i < numDeviceTypes; i++) {
                var deviceType = mixedDevicesTypes[i];
                if (Fits (config, deviceType, roundI, playerI)) {
                    config[roundI].Insert (playerI, new DeviceConfig {
                        Device = GetFirstAvailableDevice (config[roundI], deviceType)
                    });

                    if (IsLastSlot (config, roundI, playerI, numPlayers)) {
                        return true;
                    } else {
                        var nextRoundI = (roundI + 1) % config.Count;
                        var nextPlayerI = nextRoundI == 0 ? playerI + 1 : playerI;
                        if (AddNextDeviceRecursive (config, nextRoundI, nextPlayerI, numPlayers)) {
                            return true;
                        }
                    }
                    config[roundI].RemoveAt (playerI);
                }
            }

            return false;
        }

        private bool Fits (List<List<DeviceConfig>> config, DeviceTypeSpec deviceType, int roundI, int playerI) {
            if (Settings.Instance.OverrideAvailableDevices) return true;

            return FitsRound (config, deviceType, roundI, playerI) &&
                FitsPlayer (config, deviceType, roundI, playerI);
        }

        private bool FitsPlayer (List<List<DeviceConfig>> config, DeviceTypeSpec deviceType, int roundI, int playerI) {
            int numRoundsPlayerAlreadyNeedsDeviceType = config.Count (rConfig => rConfig.Count > playerI && rConfig[playerI].Device.Type == deviceType);
            int numUiModesForDeviceType = UIModeHelpers.TestUIModes.Count (uiMode => uiMode.IsForDeviceType (deviceType));
            return numRoundsPlayerAlreadyNeedsDeviceType < numUiModesForDeviceType;
        }

        private bool FitsRound (List<List<DeviceConfig>> config, DeviceTypeSpec deviceType, int roundI, int playerI) {
            int numAlreadyNeededInRound = config[roundI].Count (dConfig => dConfig != null && dConfig.Device.Type == deviceType);
            int totalNum = deviceManager.DevicesByType[deviceType].Count;
            return numAlreadyNeededInRound < totalNum;
        }

        private ConnectedDevice GetFirstAvailableDevice (List<DeviceConfig> round, DeviceTypeSpec deviceType) {
            ConnectedDevice device = null;
            if (deviceManager.DevicesByType.ContainsKey (deviceType)) {
                device = deviceManager.DevicesByType[deviceType].FirstOrDefault (d => DeviceIsUnused (d, round));
            }
            if (device == null && Settings.Instance.OverrideAvailableDevices) {
                device = deviceManager.ConnectedDevices.First (d => DeviceIsUnused (d.Value, round)).Value;
            }
            return device;
        }

        private bool DeviceIsUnused (ConnectedDevice device, List<DeviceConfig> round) {
            return !round.Exists (dc => dc.Device.Equals (device));
        }

        private bool IsLastSlot (List<List<DeviceConfig>> config, int roundI, int playerI, int numPlayers) {
            Assert.IsTrue (config != null);

            return roundI + 1 == config.Count && playerI + 1 == numPlayers;
        }

        private void PrintDeviceRoundConfig (List<List<DeviceConfig>> config) {
            var output = new StringBuilder ();
            output.AppendLine ("----- Device-Round Config -----");

            for (var i = 0; i < config[0].Count; i++) {
                foreach (var round in config) {
                    if (round.Count <= i) {
                        output.Append ("                                 ");
                    } else {
                        var devType = round[i] != null ? round[i].Device.Type.ToString () : "null";
                        var devAddress = round[i] != null ? round[i].Device.Connection.address : "null";
                        var uiMode = round[i] != null ? (int) round[i].UIMode : -1;
                        output.Append (String.Format ("[{0}:{1}:{2}]  ", devType, uiMode, devAddress));
                    }
                }
                output.AppendLine ();
            }

            output.AppendLine ("-------------------------------");
            Debug.Log (output.ToString ());
        }
    }
}