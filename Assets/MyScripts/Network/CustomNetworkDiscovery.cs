using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Network {
	public class CustomNetworkDiscovery : NetworkDiscovery {

		public static CustomNetworkDiscovery Instance { get; private set; }

		private bool connecting;

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
			}
		}

		public void StartBroadcasting () {
			if (running) return;

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
			lock (this) {
				if (connecting) return;

				connecting = true;

				NetworkManager.singleton.networkAddress = fromAddress;
				NetworkManager.singleton.StartClient ();

				StartCoroutine (DelayUtil.Do (2f, () => {
					lock (this) {
						StopBroadcasting ();
						connecting = false;
					}
				}));
			}
		}

		public void StopBroadcasting () {
			if (running) {
				StopBroadcast ();
			}
		}
	}
}