using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.Agents;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.PlayConfig.Rounds;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace CubeArena.Assets.MyScripts.Network {
	public class CustomNetworkManager : UnityEngine.Networking.NetworkManager {

		public static bool IsServer { get; private set; }

		public void Start () {
#if !UNITY_EDITOR
			networkAddress = Settings.Instance.ServerIp;
#endif

			NetworkServer.RegisterHandler (MessageIds.CustomHandleTransform_CA,
				NetworkTransform_CA.HandleTransform_CA);
			NetworkServer.RegisterHandler (MessageIds.CustomHandleTransform_CA2,
				NetworkTransform_CA2.HandleTransform_CA2);
		}

		public override void OnServerDisconnect (NetworkConnection conn) {
			DeviceManager.Instance.UnregisterDevice (conn);
		}

		public override void OnStartServer () {
			NetworkServer.RegisterHandler (MessageIds.CustomHandleTransform_CA,
				NetworkTransform_CA.HandleTransform_CA);
			NetworkServer.RegisterHandler (MessageIds.CustomHandleTransform_CA2,
				NetworkTransform_CA2.HandleTransform_CA2);

			IsServer = true;
		}

		public override void OnStopServer () {
			IsServer = false;
			DeviceManager.Instance.ResetDevices ();
		}

		override public void OnServerAddPlayer (NetworkConnection conn, short playerControllerId, NetworkReader msgReader) {
			DeviceTypeMessage msg = new DeviceTypeMessage ();
			msg.Deserialize (msgReader);

			DeviceManager.Instance.RegisterConnectedDevice (
				new ConnectedDevice (conn, playerControllerId, msg.Type, msg.Model));

			/*if (Settings.Instance.AutoStartGame) {
				FindObjectOfType<RoundManager> ().TriggerNewRound ();
			}*/
		}

		override public void OnServerAddPlayer (NetworkConnection conn, short playerControllerId) {
			Debug.LogWarning ("OnServerAddPlayer called without device info");
			if (DeviceManager.Instance.HasConnectedDevice (conn, playerControllerId)) {
				Debug.LogError ("...with unregistered device!");
			}
		}

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

			if (!IsServer) {
				GetComponent<NetworkManagerHUD> ().showGUI = false;
				GetComponent<NetworkDiscovery> ().showGUI = false;
				if (UIModeList.Instance) {
					UIModeList.Instance.SetEnabled (!Settings.Instance.DisableUIModeListOnClients);
				}
			}
		}

		public override void OnClientDisconnect (NetworkConnection conn) {
			base.OnClientDisconnect (conn);
			GetComponent<NetworkManagerHUD> ().showGUI = true;
			GetComponent<NetworkDiscovery> ().showGUI = true;
			UIModeList.Instance.SetEnabled (true);
		}
	}
}