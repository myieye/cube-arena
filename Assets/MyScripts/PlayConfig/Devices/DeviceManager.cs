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
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.PlayConfig.Devices {
	public class DeviceManager : Singleton<DeviceManager>, IDeviceManager {

		private DeviceManager () : base () {
			ConnectedDevices = new Dictionary<string, ConnectedDevice> ();
			DevicesByType = new Dictionary<DeviceTypeSpec, List<ConnectedDevice>> ();
		}

		public Dictionary<string, ConnectedDevice> ConnectedDevices { get; private set; }
		public Dictionary<DeviceTypeSpec, List<ConnectedDevice>> DevicesByType { get; private set; }

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

		public void ResetDevices () {
			if (Settings.Instance.LogDeviceConnections) {
				Debug.Log (string.Format ("Clearing connected devices. ({0})", ConnectedDevices.Count));
			}
			ConnectedDevices.Clear ();
			DevicesByType.Clear ();
		}

		public bool HasConnectedDevice (NetworkConnection conn, short controllerId) {
			var key = GenerateConnectionKey (conn, controllerId);
			return ConnectedDevices.ContainsKey (key);
		}

		private string GenerateConnectionKey (NetworkConnection conn, short playerControllerId) {
			return string.Format ("{0}:{1}:{2}", conn.connectionId, conn.address, playerControllerId);
		}

		public bool EnoughDevicesAvailable (int numPlayers) {
			var allTestDeviceTypesPresent = DeviceTypeSpecHelpers.TestDeviceTypes.All (deviceType => DevicesByType.Keys.Contains (deviceType));
			var enoughOfEachTestDeviceType = DevicesByType.All (
				pair => !pair.Key.IsTestDeviceType () || pair.Value.Count >= numPlayers / 2f);
			return allTestDeviceTypesPresent && enoughOfEachTestDeviceType;
		}
	}
}