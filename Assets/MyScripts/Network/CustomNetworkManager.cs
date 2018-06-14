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

		private void Start () {

			ConfigureNetwork ();

#if !UNITY_EDITOR
			networkAddress = Settings.Instance.ServerIp;
#endif
		}

		public override void OnServerDisconnect (NetworkConnection conn) {
			DeviceManager.Instance.UnregisterDevice (conn);
		}

		public override void OnStartServer () {
			NetworkServer.RegisterHandler (MessageIds.RelativeNetworkTransform_Server,
				RelativeNetworkTransformMessageHandler.HandleRelativeNetworkTransform_Server);

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
			client.RegisterHandler (MessageIds.RelativeNetworkTransform_Client,
				RelativeNetworkTransformMessageHandler.HandleRelativeNetworkTransform_Client);

			if (!clientLoadedScene) {
				// Ready/AddPlayer is usually triggered by a scene load completing. if no scene was loaded, then Ready/AddPlayer it here instead.
				ClientScene.Ready (conn);
				var msg = new DeviceTypeMessage {
					Type = DeviceTypeManager.DeviceType,
						Model = SystemInfo.deviceModel
				};
				ClientScene.AddPlayer (client.connection, 0, msg);
			}

			if (!IsServer) {
				GetComponent<NetworkManagerHUD> ().showGUI = false;
				GetComponent<NetworkDiscovery> ().showGUI = false;
				if (UIModeList.Instance) {
					UIModeList.Instance.SetEnabled (!Settings.Instance.DisableUIModeListOnClients);
				}
				CustomNetworkDiscovery.Instance.StopBroadcasting ();
			}
		}

		public override void OnClientDisconnect (NetworkConnection conn) {
			base.OnClientDisconnect (conn);
			GetComponent<NetworkManagerHUD> ().showGUI = true;
			GetComponent<NetworkDiscovery> ().showGUI = true;
			UIModeList.Instance.SetEnabled (true);
		}

		void ConfigureNetwork () {
			customConfig = true;

			var myConfig = connectionConfig;
			myConfig.AddChannel (QosType.Reliable);
			myConfig.AddChannel (QosType.Unreliable);
			myConfig.AddChannel (QosType.StateUpdate);
			myConfig.AddChannel (QosType.AllCostDelivery);
			myConfig.NetworkDropThreshold = 95; //95% packets that need to be dropped before connection is dropped
			myConfig.OverflowDropThreshold = 30; //30% packets that need to be dropped before sendupdate timeout is increased 
			myConfig.InitialBandwidth = 0;
			myConfig.MinUpdateTimeout = 10;
			myConfig.ConnectTimeout = 2000; // timeout before re-connect attempt will be made
			myConfig.PingTimeout = 1500; // should have more than 3 pings per disconnect timeout, and more than 5 messages per ping
			myConfig.DisconnectTimeout = 6000; // with a ping of 500 a disconnectimeout of 2000 is supposed to work well
			myConfig.PacketSize = 1470;
			myConfig.SendDelay = 2;
			myConfig.FragmentSize = 1300;
			myConfig.AcksType = ConnectionAcksType.Acks128;
			myConfig.MaxSentMessageQueueSize = 256;
			myConfig.AckDelay = 1;

			//HostTopology myTopology = new HostTopology (myConfig, 4); //up to 4 connection allowed 
			//NetworkServer.Configure (myTopology);
			//client.Configure (myTopology);
		}
	}
}