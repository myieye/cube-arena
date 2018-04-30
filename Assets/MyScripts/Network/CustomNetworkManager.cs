﻿using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.Agents;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.PlayConfig.Rounds;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
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
			if (Settings.Instance.LogDeviceConnections) {
				Debug.Log (string.Format ("Device Connected: {0} ({1})", msg.Model, msg.Type));
			}
			DeviceManager.RegisterConnectedDevice (
				new ConnectedDevice (conn, playerControllerId, msg.Type, msg.Model));

			if (Settings.Instance.AutoStartGame) {
				FindObjectOfType<RoundManager> ().TriggerNewRound ();
			}
		}

		override public void OnServerAddPlayer (NetworkConnection conn, short playerControllerId) {
			Debug.LogWarning ("OnServerAddPlayer called without device info");
			if (DeviceManager.HasConnectedDevice (conn, playerControllerId)) {
				Debug.LogError ("...with unregistered device!");
			}
		}

		private bool clientIsConnecting = false;
		/*
			Copied from NetworkManager.
			Added extra message.
		 */
		public override void OnClientConnect (NetworkConnection conn) {
			if (!clientLoadedScene) {
				// Ready/AddPlayer is usually triggered by a scene load completing. if no scene was loaded, then Ready/AddPlayer it here instead.
				ClientScene.Ready (conn);
				var msg = new DeviceTypeMessage {
					Type = DeviceTypeManager.DeviceType,
						Model = SystemInfo.deviceModel
				};
				ClientScene.AddPlayer (client.connection, 0, msg);
				//UIModeManager.Instance<UIModeMAnager> ().uiModeManager.OnClientConnect ();
			}
		}
	}
}