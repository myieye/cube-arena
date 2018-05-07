using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyPrefabs.Cursor;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.Interaction.Abstract;
using CubeArena.Assets.MyScripts.Interaction.HMD;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Helpers;
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
		public SelectAndAxesGestures SelectAndAxesGestures {
			get {
				return GameObjectUtil.FindObjectOfExactType<SelectAndAxesGestures> ();
			}
		}
		public SelectAxesAndCursorPointerGestures SelectAxesAndCursorPointerGestures {
			get {
				return GameObjectUtil.FindObjectOfExactType<SelectAxesAndCursorPointerGestures> ();
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
		private GameObject sprayBar;
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

		public CursorController.CursorMode CurrentCursorMode { get; private set; }
		public UIMode CurrentUIMode { get; private set; }

		void Start () {
			InvokeRepeating ("TryRegisterUIModeMessageHandler", 0, 0.1f);
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
			StartCoroutine (DelayUtil.Do (Settings.Instance.PassToPlayerTime, () => {
				PassToPlayerText.text = "";
				PassToPlayerText.enabled = false;
			}));
			SetUIMode (mode);
		}

		public void OnUIModeChanged (int uiMode) {
			Debug.Log ("OnUIModeChanged: " + UIModeHelpers.UIModesForCurrentDevice[uiMode]);
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
				case UIMode.Mouse:
					inputMethod = CrossPlatformInputManager.ActiveInputMethod.Hardware;
					CurrentCursorMode = CursorController.CursorMode.Mouse;
					sprayMoveButton.SetActive (true);
					sprayMoveButton.GetComponent<RectTransform> ().anchoredPosition = Positions.CanvasRightBottom;
					break;
				case UIMode.HHD1_Camera:
					joystick.SetActive (true);
					selectButton.SetActive (true);
					sprayMoveButton.SetActive (true);
					CurrentCursorMode = CursorController.CursorMode.Camera;
					sprayMoveButton.GetComponent<RectTransform> ().anchoredPosition = Positions.CanvasRightMiddle;
					break;
				case UIMode.HHD2_TouchAndDrag:
					joystick.SetActive (true);
					touchpad.SetActive (true);
					sprayMoveButton.SetActive (true);
					CurrentCursorMode = CursorController.CursorMode.Touch;
					sprayMoveButton.GetComponent<RectTransform> ().anchoredPosition = Positions.CanvasRightBottom;
					break;
				case UIMode.HHD3_Gestures:
					touchpad.SetActive (true);
					sprayMoveButton.SetActive (true);
					CurrentCursorMode = CursorController.CursorMode.Touch;
					sprayMoveButton.GetComponent<RectTransform> ().anchoredPosition = Positions.CanvasRightBottom;
					break;
#if (UNITY_WSA || UNITY_EDITOR)
				case UIMode.HMD4_GazeAndClicker:
					SelectAndAxesGestures.enabled = true;
					CurrentCursorMode = CursorController.CursorMode.Camera;
					break;
				case UIMode.HMD5_Gaze__AirTap_Drag_And_Clicker_Rotate:
					SelectAxesAndCursorPointerGestures.enabled = true;
					CurrentCursorMode = CursorController.CursorMode.Pointer;
					break;
#endif
			}
			CrossPlatformInputManager.SwitchActiveInputMethod (inputMethod);
		}

		public void SetPlayerUIModes (List<Players.NetworkPlayer> players) {
			foreach (var player in players) {
				var msg = new UIModeMessage {
					UIMode = player.DeviceConfig.UIMode,
						PlayerNum = player.PlayerNum
				};
				NetworkServer.SendToClient (player.DeviceConfig.Device.Connection.connectionId,
					MessageIds.SetUIMode, msg);
			}
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
			if (sprayMoveButton) {
				sprayMoveButton.SetActive (false);
			}

#if (UNITY_WSA || UNITY_EDITOR)
			if (SelectAndAxesGestures) {
				SelectAndAxesGestures.enabled = false;
			}
			if (SelectAxesAndCursorPointerGestures) {
				SelectAxesAndCursorPointerGestures.enabled = false;
			}
#endif
		}

		public static bool InMode (UIMode mode) {
			return Instance<UIModeManager> ().CurrentUIMode.Equals (mode);
		}
	}
}