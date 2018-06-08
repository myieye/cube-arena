using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Network {
	public class NetworkGUIToggle : MonoBehaviour {

		[SerializeField]
		private int touchCount;
		private bool valid;
		private NetworkManagerHUD networkManagerHUD;
		private NetworkDiscovery networkDiscovery;
		private bool toggleNetworkManagerHUD;
		private bool toggleNetworkDiscovery;

		private bool ToggleActive {
			get {
#if UNITY_EDITOR || UNITY_STANDALONE
				return Input.GetMouseButtonDown (1);
#elif UNITY_ANDROID
				return Input.touchCount >= touchCount;
#elif UNITY_WSA
				return false;
#endif
			}
		}

		private void Start () {
#if UNITY_WSA && !UNITY_EDITOR
			Destroy (this);
#endif

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
			if (valid && ToggleActive) {
				ToggleGUI ();
				valid = false;
			} else if (!ToggleActive) {
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