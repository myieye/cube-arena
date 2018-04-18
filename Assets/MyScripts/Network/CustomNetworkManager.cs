using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.Agents;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.PlayConfig.Rounds;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace CubeArena.Assets.MyScripts.Network {
	public class CustomNetworkManager : UnityEngine.Networking.NetworkManager {

		private DeviceManager _deviceManager;
		public DeviceManager DeviceManager {
			get {
				if (!_deviceManager) {
					_deviceManager = FindObjectOfType<DeviceManager> ();
				}
				return _deviceManager;
			}
		}

		override public void OnServerAddPlayer (NetworkConnection conn, short playerControllerId, NetworkReader msgReader) {
			DeviceTypeMessage msg = new DeviceTypeMessage ();
			msg.Deserialize (msgReader);
			Debug.Log("Device Connected: " + msg.Model);
			DeviceManager.RegisterConnectedDevice (
				new ConnectedDevice(conn, playerControllerId, msg.Type, msg.Model));
		}

		override public void OnServerAddPlayer (NetworkConnection conn, short playerControllerId) {
			Debug.LogWarning ("OnServerAddPlayer called without device type");
		}

		/*
			Copied from NetworkManager.
			Added extra message.
		 */
		public override void OnClientConnect (NetworkConnection conn) {
			if (!clientLoadedScene) {
				// Ready/AddPlayer is usually triggered by a scene load completing. if no scene was loaded, then Ready/AddPlayer it here instead.
				ClientScene.Ready (conn);
				var msg = DeviceManager.BuildDeviceTypeMessage();
				ClientScene.AddPlayer (client.connection, 0, msg);
			}
		}
	}
}