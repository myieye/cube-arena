using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Network {
	public class CustomNetworkDiscovery : NetworkDiscovery {

		public static CustomNetworkDiscovery Instance { get; private set; }

		private bool StartAutomatically {
			get {
				return (DeviceTypeManager.DeviceType.IsServerDeviceType () &&
						Settings.Instance.StartServerAutomatically) ||
					(!DeviceTypeManager.DeviceType.IsServerDeviceType () &&
						Settings.Instance.StartClientAutomatically);
			}
		}

		void Start () {
			Instance = this;

			if (StartAutomatically) {
				StartBroadcasting ();
				//StartBroadcasting ();
			}
		}

		public void StartBroadcasting () {
			InitializeNetworkDiscovery ();

			if (DeviceTypeManager.DeviceType.IsServerDeviceType ()) {
				StartAsServer ();
				NetworkManager.singleton.StartHost ();
			} else if (!DeviceTypeManager.DeviceType.IsServerDeviceType ()) {
				StartAsClient ();
			}
		}

		public bool InitializeNetworkDiscovery () {
			return Initialize ();
		}

		public override void OnReceivedBroadcast (string fromAddress, string data) {
			NetworkManager.singleton.networkAddress = fromAddress;
			NetworkManager.singleton.StartClient ();
			StopBroadcasting ();
		}

		public void StopBroadcasting () {
			StopBroadcast ();
		}
	}
}