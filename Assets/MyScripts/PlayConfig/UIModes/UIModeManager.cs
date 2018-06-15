using System.Collections.Generic;
using CACursorMode = CubeArena.Assets.MyPrefabs.Cursor.CursorMode;
using CubeArena.Assets.MyPrefabs.Cursor;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Interaction.HMD.Gestures;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.PlayConfig.UIModes {
	public class UIModeManager : NetworkBehaviourSingleton {

		private GameObject Cursor {
			get {
				var component = GameObjectUtil.FindLocalAuthoritativeObject<CursorController> ();
				return component ? component.gameObject : null;
			}
		}
		private CursorController StandardCursorController {
			get {
				return Cursor.GetComponentOfExactType<CursorController> ();
			}
		}
		private CursorController TouchCursorController {
			get {
				return Cursor.GetComponentOfExactType<TouchCursorController> ();
			}
		}

#if (UNITY_WSA || UNITY_EDITOR)
		private HMD_UI1 HMD_UI1 {
			get {
				return FindObjectOfType<HMD_UI1> ();
			}
		}
		private HMD_UI2 HMD_UI2 {
			get {
				return FindObjectOfType<HMD_UI2> ();
			}
		}
		private HMD_UI3 HMD_UI3 {
			get {
				return FindObjectOfType<HMD_UI3> ();
			}
		}
		private HMD_SprayToggle HMD_SprayToggle {
			get {
				return FindObjectOfType<HMD_SprayToggle> ();
			}
		}
#endif

		[SerializeField]
		private GameObject joystick;
		[SerializeField]
		private GameObject selectButton;

		[SerializeField]
		private GameObject touchpad;
		[SerializeField]
		private GameObject sprayMoveButton;
		[SerializeField]
		private UIModeList uiModeList;
		[SerializeField]
		private GameObject controls;
		private GameObject Controls {
			get {
				if (!controls) {
					controls = GameObject.Find (Names.Controls);
				}
				return controls;
			}
		}

		[SerializeField]
		private UIModeARObject twoDTranslationPlane;

		private UIModeARObject TwoDTranslationPlane {
			get {
				if (!twoDTranslationPlane) {
					twoDTranslationPlane =
						GameObject.Find (Names.TwoDTranslationPlane)
						.GetComponent<UIModeARObject> ();
				}
				return twoDTranslationPlane;
			}
		}

		[SerializeField]
		private UnityEngine.UI.Text passToPlayerText;
		private UnityEngine.UI.Text PassToPlayerText {
			get {
				if (!passToPlayerText) {
					passToPlayerText = GameObject.Find (Names.PassToPlayerText)
						.GetComponent<UnityEngine.UI.Text> ();
				}
				return passToPlayerText;
			}
		}

		public CACursorMode CurrentCursorMode { get; private set; }
		public UIMode CurrentUIMode { get; private set; }
		private static List<UIModeChangedListener> uiModeChangedListeners;

		static UIModeManager () {
			uiModeChangedListeners = new List<UIModeChangedListener> ();
		}

		private void OnEnable () {
			InvokeRepeating ("TryRegisterUIModeMessageHandler", 0, 0.1f);
			SetUIMode (UIMode.None);
			uiModeList.SetVisible (false || isServer);
		}

		private void OnDisable () {
			if (uiModeList) {
				uiModeList.SetVisible (false || isServer);
			}
			if (controls) {
				controls.SetActive (false);
			}
		}

		private void TryRegisterUIModeMessageHandler () {
			if (NetworkManager.singleton != null && NetworkManager.singleton.client != null) {
				NetworkManager.singleton.client.RegisterHandler (
					MessageIds.SetUIMode, OnUIModeMessage);
				CancelInvoke ("TryRegisterUIModeMessageHandler");
			}
		}

		private void OnUIModeMessage (NetworkMessage netMsg) {
			var modeMsg = netMsg.ReadMessage<UIModeMessage> ();

			if (modeMsg.ForceUserStudySettings && !isServer) {
				Settings.Instance.EnableUserStudySettings ();
			}

			UIMode mode;
			if (Settings.Instance.ForceTestUIMode) {
				mode = Settings.Instance.TestUIMode;
			} else if (Settings.Instance.ForceDefaultUIMode) {
				mode = Settings.Instance.DefaultUIMode;
			} else {
				mode = modeMsg.UIMode;
			}

			if (modeMsg.SwappingDevices) {
				PassToPlayerText.enabled = true;
				PassToPlayerText.text = Text.PassToPlayerText (modeMsg.PlayerNum, mode);
			}

			uiModeList.SetVisible (false);

			StartCoroutine (DelayUtil.Do (modeMsg.Delay, () => {
				PassToPlayerText.text = "";
				PassToPlayerText.enabled = false;
				uiModeList.SetVisible (mode != UIMode.None || isServer);
				SetUIMode (mode);
			}));

			uiModeList.SetEnabled (!modeMsg.DisableUIModeList || isServer);
		}

		public void OnUIModeChanged (int uiMode) {
			SetUIMode (UIModeHelpers.UIModesForCurrentDevice[uiMode], force : false);
		}

		private void SetUIMode (UIMode mode, bool force = true) {
			Controls.SetActive (true);

			if (mode == CurrentUIMode && !force) {
				return;
			}
			if (Settings.Instance.LogUIMode) {
				Debug.Log ("SetUIMode: " + mode);
			}

			CurrentUIMode = mode;
			uiModeList.RefreshSelectedUIMode ();
			DisableAll ();
			var inputMethod = CrossPlatformInputManager.ActiveInputMethod.Touch;

			switch (mode) {
				case UIMode.None:
					Controls.SetActive (false);
					break;
				case UIMode.Mouse:
					inputMethod = CrossPlatformInputManager.ActiveInputMethod.Hardware;
					CurrentCursorMode = CACursorMode.Mouse;
					sprayMoveButton.GetComponent<RectTransform> ().anchoredPosition = Positions.CanvasRightBottom;
					break;
				case UIMode.HHD1_Camera:
					joystick.SetActive (true);
					selectButton.SetActive (true);
					CurrentCursorMode = CACursorMode.Camera;
					sprayMoveButton.GetComponent<RectTransform> ().anchoredPosition = Positions.CanvasRightMiddle;
					break;
				case UIMode.HHD2_Touch:
					joystick.SetActive (true);
					touchpad.SetActive (true);
					CurrentCursorMode = CACursorMode.Touch;
					sprayMoveButton.GetComponent<RectTransform> ().anchoredPosition = Positions.CanvasRightBottom;
					break;
				case UIMode.HHD3_Gestures:
					touchpad.SetActive (true);
					CurrentCursorMode = CACursorMode.Touch;
					sprayMoveButton.GetComponent<RectTransform> ().anchoredPosition = Positions.CanvasRightBottom;
					break;
#if (UNITY_WSA || UNITY_EDITOR)
				case UIMode.HMD1_Gaze:
					HMD_UI1.enabled = true;
					HMD_SprayToggle.enabled = true;
					CurrentCursorMode = CACursorMode.Camera;
					sprayMoveButton.GetComponent<RectTransform> ().anchoredPosition = Positions.CanvasRightBottom;
					break;
				case UIMode.HMD2_Point:
					HMD_UI2.enabled = true;
					HMD_SprayToggle.enabled = true;
					CurrentCursorMode = CACursorMode.Pointer;
					sprayMoveButton.GetComponent<RectTransform> ().anchoredPosition = Positions.CanvasRightBottom;
					break;
				case UIMode.HMD3_Translate:
					HMD_UI3.enabled = true;
					HMD_SprayToggle.enabled = true;
					CurrentCursorMode = CACursorMode.Translate;
					sprayMoveButton.GetComponent<RectTransform> ().anchoredPosition = Positions.CanvasRightBottom;
					break;
#endif
			}
			CrossPlatformInputManager.SwitchActiveInputMethod (inputMethod);

			if (Cursor) {
				StandardCursorController.enabled = !InTouchMode;
				TouchCursorController.enabled = InTouchMode;
			}

			NotifyUIModeChangesListeners ();
		}

		public void SetPlayerUIModes (List<Players.NetworkPlayer> players, float delay, bool swappingDevices) {
			foreach (var player in players) {
				SetPlayerUIMode (player, player.DeviceConfig.UIMode, delay, swappingDevices);
			}
		}

		public void DisablePlayerUIs (List<Players.NetworkPlayer> players) {
			foreach (var player in players) {
				SetPlayerUIMode (player, UIMode.None, 0, false);
			}
		}

		private void SetPlayerUIMode (Players.NetworkPlayer player, UIMode uiMode, float delay, bool swappingDevices) {
			var msg = new UIModeMessage {
				UIMode = uiMode, PlayerNum = player.PlayerNum,
					SwappingDevices = swappingDevices,
					Delay = delay,
					DisableUIModeList = Settings.Instance.DisableUIModeListOnClients,
					ForceUserStudySettings = Settings.Instance.ForceUserStudySettings
			};
			NetworkServer.SendToClient (player.DeviceConfig.Device.Connection.connectionId,
				MessageIds.SetUIMode, msg);
		}

		private void DisableAll () {
			if (joystick) {
				joystick.SetActive (false);
			}
			if (selectButton) {
				selectButton.SetActive (false);
			}
			if (touchpad) {
				touchpad.SetActive (false);
			}

#if (UNITY_WSA || UNITY_EDITOR)
			if (HMD_UI1) {
				HMD_UI1.enabled = false;
			}
			if (HMD_UI2) {
				HMD_UI2.enabled = false;
			}
			if (HMD_UI3) {
				HMD_UI3.enabled = false;
			}
			if (HMD_SprayToggle) {
				HMD_SprayToggle.enabled = false;
			}
#endif
		}

		public static void RegisterUIModeChangedListener (UIModeChangedListener uiModeChangedListener) {
			uiModeChangedListeners.Add (uiModeChangedListener);
		}

		public static void UnregisterUIModeChangedListener (UIModeChangedListener uiModeChangedListener) {
			uiModeChangedListeners.Remove (uiModeChangedListener);
		}

		private void NotifyUIModeChangesListeners () {
			for (var i = 0; i < uiModeChangedListeners.Count; i++) {
				var listener = uiModeChangedListeners[i];
				if (listener == null || !listener.gameObject) {
					uiModeChangedListeners.RemoveAt (i);
					i--;
				} else {
					listener.OnUIModeChanged (CurrentUIMode);
				}
			}
		}

		public static bool InUIMode (UIMode mode) {
			return Instance<UIModeManager> ().CurrentUIMode.Equals (mode);
		}

		public static bool InCursorMode (CACursorMode cursorMode) {
			return Instance<UIModeManager> ().CurrentCursorMode == cursorMode;
		}

		public static bool InTouchMode {
			get {
				return InCursorMode (CACursorMode.Touch);
			}
		}
		public static bool InPointerMode {
			get {
				return InCursorMode (CACursorMode.Pointer);
			}
		}
	}
}