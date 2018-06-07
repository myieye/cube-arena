using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Network {
	public class TouchNetworkGUIToggle : MonoBehaviour {

		[SerializeField]
		private int touchCount;
		private bool valid;
		private NetworkManagerHUD networkManagerHUD;
		private NetworkDiscovery networkDiscovery;
		private bool toggleNetworkManagerHUD;
		private bool toggleNetworkDiscovery;

		private void Start () {
			if (!DeviceTypeManager.IsDeviceType (DeviceTypeSpec.Android)) {
				Destroy (this);
			}

			valid = true;
			networkManagerHUD = GetComponent<NetworkManagerHUD> ();
			networkDiscovery = GetComponent<NetworkDiscovery> ();

			if (!networkManagerHUD && !networkDiscovery) {
				Debug.LogWarning ("No Network GUI components found. Destroying TouchNetworkGUIToggle");
				Destroy (this);
			}

			toggleNetworkManagerHUD = networkManagerHUD && networkManagerHUD.showGUI;
			toggleNetworkDiscovery = networkDiscovery && networkDiscovery.showGUI;
		}

		private void Update () {
			if (valid && Input.touchCount >= touchCount) {
				ToggleGUI ();
				valid = false;
			} else if (Input.touchCount < touchCount) {
				valid = true;
			}
		}

		private void ToggleGUI () {
			bool show =
				toggleNetworkManagerHUD ? !networkManagerHUD.showGUI :
				toggleNetworkDiscovery ? !networkDiscovery.showGUI : false;

			if (toggleNetworkManagerHUD) {
				networkManagerHUD.showGUI = show;
			}

			if (toggleNetworkDiscovery) {
				networkDiscovery.showGUI = show;
			}
		}
	}
}