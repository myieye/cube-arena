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
		public Dictionary<DeviceTypeSpec, List<ConnectedDevice>> DevicesByType { get; private set; }

		void Start () {
			ConnectedDevices = new Dictionary<string, ConnectedDevice> ();
			DevicesByType = new Dictionary<DeviceTypeSpec, List<ConnectedDevice>> ();
		}

		[Server]
		public void RegisterConnectedDevice (ConnectedDevice connectedDevice) {
			var key = GenerateConnectionKey (connectedDevice.Connection, connectedDevice.ControllerId);
			if (!ConnectedDevices.ContainsKey (key)) {

				if (Settings.Instance.LogDeviceConnections) {
					Debug.Log (string.Format ("Device Connected: {0} ({1})", connectedDevice.Model, connectedDevice.Type));
				}

				DataService.Instance.SaveDeviceIfNewModel (connectedDevice);
				//var netPlayer = PlayerManager.Instance.AddPlayer (device.Connection, playerControllerId);
				ConnectedDevices[key] = connectedDevice;
				if (!DevicesByType.ContainsKey (connectedDevice.Type)) {
					DevicesByType.Add (connectedDevice.Type, new List<ConnectedDevice> ());
				}
				DevicesByType[connectedDevice.Type].Add (connectedDevice);
			}
		}

		[Server]
		public void UnregisterDevice (NetworkConnection conn) {
			var connectedDevicePair = ConnectedDevices.FirstOrDefault (dev => dev.Value.Connection.connectionId == conn.connectionId);
			var connectedDevice = connectedDevicePair.Value;
			if (connectedDevice != null) {
				if (Settings.Instance.LogDeviceConnections) {
					Debug.Log (string.Format ("Device Disonnected: {0} ({1})", connectedDevice.Model, connectedDevice.Type));
				}
				ConnectedDevices.Remove (connectedDevicePair.Key);
			} else {
				Debug.Log ("Didn't find disconnecting device");
			}
		}

		[Server]
		public void ResetDevices () {
			if (Settings.Instance.LogDeviceConnections) {
				Debug.Log (string.Format ("Clearing connected devices. ({0})", ConnectedDevices.Count));
			}
			ConnectedDevices.Clear ();
			DevicesByType.Clear ();
		}

		[Server]
		public bool HasConnectedDevice (NetworkConnection conn, short controllerId) {
			var key = GenerateConnectionKey (conn, controllerId);
			return ConnectedDevices.ContainsKey (key);
		}

		[Server]
		private string GenerateConnectionKey (NetworkConnection conn, short playerControllerId) {
			return string.Format ("{0}:{1}:{2}", conn.connectionId, conn.address, playerControllerId);
		}

		[Server]
		public List<List<DeviceConfig>> GenerateDeviceRoundConfigs (int numPlayers) {
			var numRounds = UIModeHelpers.UIModes.Count;
			var config = new List<List<DeviceConfig>> ();

			int tries = 0;
			bool valid = false;
			while (!valid) {
				tries++;
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

			if (Settings.Instance.LogDeviceRoundConfig) {
				Debug.Log (String.Format ("Generated device-round config: {0}:{1}:{2}",
					numPlayers, ConnectedDevices.Count, DevicesByType.Count));
				Debug.Log ("Num tries: " + tries);
			}
			return config;
		}

		public bool EnoughDevicesAvailable (int numPlayers) {
			return DevicesByType.Count == 2 &&
				DevicesByType.All (pair => pair.Value.Count >= numPlayers / 2);
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
			var modes = new List<UIMode> (UIModeHelpers.UIModes);
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

		private ConnectedDevice GetFirstAvailableDevice (List<DeviceConfig> round, DeviceTypeSpec deviceType) {
			ConnectedDevice device = null;
			if (DevicesByType.ContainsKey (deviceType)) {
				device = DevicesByType[deviceType].FirstOrDefault (d => DeviceIsUnused (d, round));
			}
			if (device == null && Settings.Instance.OverrideAvailableDevices) {
				device = ConnectedDevices.First (d => DeviceIsUnused (d.Value, round)).Value;
			}
			return device;
		}

		private bool DeviceIsUnused (ConnectedDevice device, List<DeviceConfig> round) {
			return !round.Exists (dc => dc.Device.Equals (device));
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