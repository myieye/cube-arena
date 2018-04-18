using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CubeArena.Assets.MyScripts.Logging.DAL;
using CubeArena.Assets.MyScripts.Logging.DAL.Models;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.PlayConfig.Devices {
	public class DeviceManager : NetworkBehaviourSingleton {

		public Dictionary<string, ConnectedDevice> ConnectedDevices { get; private set; }
		public Dictionary<DeviceType, List<ConnectedDevice>> DevicesByType { get; private set; }

		[Server]
		void Start () {
			ConnectedDevices = new Dictionary<string, ConnectedDevice> ();
			DevicesByType = new Dictionary<DeviceType, List<ConnectedDevice>> ();
		}

		[Server]
		public void RegisterConnectedDevice (ConnectedDevice connectedDevice) {
			var key = GenerateConnectionKey (connectedDevice.Connection, connectedDevice.ControllerId);
			if (!ConnectedDevices.ContainsKey (key)) {
				DataService.Instance.SaveDeviceIfNewModel(connectedDevice);
				//var netPlayer = PlayerManager.Instance.AddPlayer (device.Connection, playerControllerId);
				ConnectedDevices[key] = connectedDevice;
				if (!DevicesByType.ContainsKey (connectedDevice.Type)) {
					DevicesByType.Add (connectedDevice.Type, new List<ConnectedDevice> ());
				}
				DevicesByType[connectedDevice.Type].Add (connectedDevice);
			}
		}

		[Server]
		private string GenerateConnectionKey (NetworkConnection conn, short playerControllerId) {
			return string.Format ("{0}:{1}:{2}", conn.connectionId, conn.address, playerControllerId);
		}

		[Server]
		public List<List<DeviceConfig>> GenerateDeviceRoundConfigs (int numPlayers) {
			Debug.Log(String.Format("Generating: {0}:{1}:{2}", numPlayers, ConnectedDevices.Count, DevicesByType.Count));
			var modes = UIModeManager.GetUIModes ();
			var numRounds = modes.Count;
			var config = new List<List<DeviceConfig>> ();

			bool valid = false;
			while (!valid) {
				config.Clear ();
				for (var i = 0; i < numRounds; i++) {
					config.Add (new List<DeviceConfig> ());
				}

				valid = true;
				for (var i = 0; i < numPlayers && valid; i++) {
					valid = AddPlayerDeviceConfig (config);
				}

				//valid = ValidatePlayerRoundConfig (config);
				if (Settings.Instance.LogDeviceRoundConfig) {
					PrintDeviceRoundConfig (config, valid);
				}
			}
			return config;
		}

		public DeviceTypeMessage BuildDeviceTypeMessage () {
			return new DeviceTypeMessage {
				Type = SystemInfo.deviceType,
					Model = SystemInfo.deviceModel
			};
		}

		private bool AddPlayerDeviceConfig (List<List<DeviceConfig>> config) {
			var playerModes = GeneratePlayerModeConfig ();
			foreach (var round in config) {
				var mode = playerModes.First ();
				playerModes.Remove (mode);
				var device = GetFirstAvailableDevice (round, mode.GetDeviceType ());
				if (device == null) {
					return false;
				} else {
					round.Add (new DeviceConfig (device, mode));
				}
			}
			return true;
		}

		private List<UIMode> GeneratePlayerModeConfig () {
			var modes = UIModeManager.GetUIModes ();
			return modes.OrderBy (m => UnityEngine.Random.value).ToList ();
		}

		private void PrintDeviceRoundConfig (List<List<DeviceConfig>> config, bool valid) {
			var output = new StringBuilder ();
			output.AppendLine ("----- Device-Round Config -----");
			output.AppendLine ("VALID: " + valid);

			for (var i = 0; i < config[0].Count; i++) {
				foreach (var round in config) {
					var devType = round[i] != null ? round[i].Device.Type.ToString () : "null";
					var uiMode = round[i] != null ? round[i].UIMode.ToString () : "null";
					output.Append (String.Format ("({0}:{1})  ", devType, uiMode));
				}
				output.AppendLine ();
			}

			Debug.Log (output.ToString ());
		}

		private ConnectedDevice GetFirstAvailableDevice (List<DeviceConfig> round, DeviceType deviceType) {
			Debug.Log("GetDevice: " + deviceType);
			Debug.Log("Device Types: " + DevicesByType.Count + " " +
				(DevicesByType.Any() ? DevicesByType.First().Key.ToString() : "null"));
			return DevicesByType[deviceType].FirstOrDefault (d => !round.Exists (dc => dc.Device.Equals (d)));
		}

		private T RemoveFirst<T> (List<T> list) {
			var item = list.First ();
			list.Remove (item);
			return item;
		}

		/*private bool ValidatePlayerRoundConfig (List<List<DeviceConfig>> config) {
			var deviceTypes = DevicesByType.Count;
			foreach (var round in config) {
				foreach (var device in DevicesByType) {
					if (device.Value.Count < round.Count (devConfig => devConfig.DeviceType.Equals (device.Key))) {
						return false;
					}
				}
			}
			return true;
		}*/
	}
}