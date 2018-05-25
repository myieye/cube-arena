using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyPrefabs.Cursor;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.Interaction.Abstract;
using CubeArena.Assets.MyScripts.Interaction.HMD.Gestures;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.PlayConfig.UIModes {
	public class UIModeManager : NetworkBehaviourSingleton {

		public RaycastCubeMover RaycastCubeMover {
			get {
				return FindObjectOfType<RaycastCubeMover> ();
			}
		}
		public GestureCubeMover GestureCubeMover {
			get {
				return FindObjectOfType<GestureCubeMover> ();
			}
		}

#if (UNITY_WSA || UNITY_EDITOR)
		public HMD_UI1 HMD_UI1 {
			get {
				return FindObjectOfType<HMD_UI1> ();
			}
		}
		public HMD_UI2 HMD_UI2 {
			get {
				return FindObjectOfType<HMD_UI2> ();
			}
		}
		public HMD_UI3 HMD_UI3 {
			get {
				return FindObjectOfType<HMD_UI3> ();
			}
		}
		public HMD_SprayToggle HMD_SprayToggle {
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

		public CursorMode CurrentCursorMode { get; private set; }
		public UIMode CurrentUIMode { get; private set; }

		public void OnEnable () {
			InvokeRepeating ("TryRegisterUIModeMessageHandler", 0, 0.1f);
			SetUIMode (UIMode.None);
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
			UIMode mode;
			if (Settings.Instance.ForceTestUIMode) {
				mode = Settings.Instance.TestUIMode;
			} else if (Settings.Instance.ForceDefaultUIMode) {
				mode = Settings.Instance.DefaultUIMode;
			} else {
				mode = modeMsg.UIMode;
			}
			PassToPlayerText.enabled = true;
			PassToPlayerText.text = Text.PassToPlayerText (modeMsg.PlayerNum);
			StartCoroutine (DelayUtil.Do (modeMsg.PassToPlayerTime, () => {
				PassToPlayerText.text = "";
				PassToPlayerText.enabled = false;
			}));
			SetUIMode (mode);
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
			TwoDTranslationPlane.OnUIModeChanged (CurrentUIMode);

			switch (mode) {
				case UIMode.None:
					Controls.SetActive (false);
					break;
				case UIMode.Mouse:
					inputMethod = CrossPlatformInputManager.ActiveInputMethod.Hardware;
					CurrentCursorMode = CursorMode.Mouse;
					//sprayMoveButton.SetActive (true);
					sprayMoveButton.GetComponent<RectTransform> ().anchoredPosition = Positions.CanvasRightBottom;
					break;
				case UIMode.HHD1_Camera:
					joystick.SetActive (true);
					selectButton.SetActive (true);
					//sprayMoveButton.SetActive (true);
					CurrentCursorMode = CursorMode.Camera;
					sprayMoveButton.GetComponent<RectTransform> ().anchoredPosition = Positions.CanvasRightMiddle;
					break;
				case UIMode.HHD2_Touch:
					joystick.SetActive (true);
					touchpad.SetActive (true);
					//sprayMoveButton.SetActive (true);
					CurrentCursorMode = CursorMode.Touch;
					sprayMoveButton.GetComponent<RectTransform> ().anchoredPosition = Positions.CanvasRightBottom;
					break;
				case UIMode.HHD3_Gestures:
					touchpad.SetActive (true);
					//sprayMoveButton.SetActive (true);
					CurrentCursorMode = CursorMode.Touch;
					sprayMoveButton.GetComponent<RectTransform> ().anchoredPosition = Positions.CanvasRightBottom;
					break;
#if (UNITY_WSA || UNITY_EDITOR)
				case UIMode.HMD1_Gaze:
					HMD_UI1.enabled = true;
					HMD_SprayToggle.enabled = true;
					CurrentCursorMode = CursorMode.Camera;
					sprayMoveButton.GetComponent<RectTransform> ().anchoredPosition = Positions.CanvasRightBottom;
					break;
				case UIMode.HMD2_Point:
					HMD_UI2.enabled = true;
					HMD_SprayToggle.enabled = true;
					CurrentCursorMode = CursorMode.Pointer;
					sprayMoveButton.GetComponent<RectTransform> ().anchoredPosition = Positions.CanvasRightBottom;
					break;
				case UIMode.HMD3_Translate:
					HMD_UI3.enabled = true;
					HMD_SprayToggle.enabled = true;
					CurrentCursorMode = CursorMode.Translate;
					sprayMoveButton.GetComponent<RectTransform> ().anchoredPosition = Positions.CanvasRightBottom;
					break;
#endif
			}
			CrossPlatformInputManager.SwitchActiveInputMethod (inputMethod);
		}

		/*
		public bool InManipulationMode () {
			switch (CurrentUIMode) {
				case UIMode.HMD2_Point:
				case UIMode.HMD3_Translate:
					return true;
				default:
					return false;
			}
		} */

		public void SetPlayerUIModes (List<Players.NetworkPlayer> players) {
			foreach (var player in players) {
				SetPlayerUIMode (player, player.DeviceConfig.UIMode);
			}
		}

		public void DisablePlayerUIs (List<Players.NetworkPlayer> players) {
			foreach (var player in players) {
				SetPlayerUIMode (player, UIMode.None);
			}
		}

		private void SetPlayerUIMode (Players.NetworkPlayer player, UIMode uiMode) {
			var msg = new UIModeMessage {
				UIMode = uiMode, PlayerNum = player.PlayerNum,
					PassToPlayerTime = Settings.Instance.PassToPlayerTime
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
			//if (sprayMoveButton) {
			//	sprayMoveButton.SetActive (false);
			//}

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

		public static bool InUIMode (UIMode mode) {
			return Instance<UIModeManager> ().CurrentUIMode.Equals (mode);
		}

		public static bool InCursorMode (CursorMode cursorMode) {
			return Instance<UIModeManager> ().CurrentCursorMode == cursorMode;
		}
	}
}