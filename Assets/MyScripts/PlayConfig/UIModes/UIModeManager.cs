using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyPrefabs.Cursor;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.Interaction.HMD;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.PlayConfig.UIModes {
	public class UIModeManager : NetworkBehaviour {

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
				return FindObjectOfType<SelectAndAxesGestures> ();
			}
		}
		public SelectAxesAndCursorPointerGestures SelectAxesAndCursorPointerGestures {
			get {
				return FindObjectOfType<SelectAxesAndCursorPointerGestures> ();
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
		private GameObject twoDTranslationPlane;
		[SerializeField]
		private UIModeList uiModeList;

		public CursorController.CursorMode CurrentCursorMode { get; private set; }
		public UIMode CurrentUIMode { get; private set; }
		public static UIModeManager Instance { get; private set; }

		[SerializeField]
		private UIMode defaultUIMode;
		private UIMode[] modes = (UIMode[]) Enum.GetValues (typeof (UIMode));

		void Awake () {
			if (Instance) {
				Destroy (this);
				return;
			}

			Instance = this;
		}

		void Start () {
			SetUIMode (defaultUIMode);
		}

		public override void OnStartLocalPlayer () {
			base.OnStartLocalPlayer ();
			NetworkManager.singleton.client.RegisterHandler (
				MessageIds.SetUIMode,
				netMsg => SetUIMode (netMsg.ReadMessage<UIModeMessage> ().UIMode));
		}

		public void OnUIModeChanged (int uiMode) {
			SetUIMode (modes[uiMode]);
		}

		private void SetUIMode (UIMode mode) {
			CurrentUIMode = mode;
			uiModeList.RefreshSelectedUIMode ();
			DisableAll ();
			switch (mode) {
				case UIMode.Mouse:
					CurrentCursorMode = CursorController.CursorMode.Mouse;
					CrossPlatformInputManager.SwitchActiveInputMethod (
						CrossPlatformInputManager.ActiveInputMethod.Hardware);
					//RaycastCubeMover.enabled = false;
					//GestureCubeMover.enabled = true;
					break;
				case UIMode.HHD1_Camera:
					CrossPlatformInputManager.SwitchActiveInputMethod (
						CrossPlatformInputManager.ActiveInputMethod.Touch);
					joystick.SetActive (true);
					selectButton.SetActive (true);
					CurrentCursorMode = CursorController.CursorMode.Camera;
					break;
				case UIMode.HHD2_TouchAndDrag:
					CrossPlatformInputManager.SwitchActiveInputMethod (
						CrossPlatformInputManager.ActiveInputMethod.Touch);
					joystick.SetActive (true);
					touchpad.SetActive (true);
					CurrentCursorMode = CursorController.CursorMode.Touch;
					break;
				case UIMode.HHD3_Gestures:
					CrossPlatformInputManager.SwitchActiveInputMethod (
						CrossPlatformInputManager.ActiveInputMethod.Touch);
					touchpad.SetActive (true);
					twoDTranslationPlane.SetActive (true);
					CurrentCursorMode = CursorController.CursorMode.Touch;
					break;
#if (UNITY_WSA || UNITY_EDITOR)
				case UIMode.HMD4_GazeAndClicker:
					SelectAndAxesGestures.enabled = true;
					CurrentCursorMode = CursorController.CursorMode.Camera;
					// TODO ... wrong input method?
					CrossPlatformInputManager.SwitchActiveInputMethod (
						CrossPlatformInputManager.ActiveInputMethod.Touch);
					break;
				case UIMode.HMD5_Gaze__AirTap_Drag_And_Clicker_Rotate:
					SelectAxesAndCursorPointerGestures.enabled = true;
					CurrentCursorMode = CursorController.CursorMode.Pointer;
					// TODO ... wrong input method?
					CrossPlatformInputManager.SwitchActiveInputMethod (
						CrossPlatformInputManager.ActiveInputMethod.Touch);
					break;
#endif
			}
		}

		public void SetPlayerUIModes (List<Players.NetworkPlayer> players) {
			foreach (var player in players) {
				NetworkServer.SendToClientOfPlayer (player.PlayerGameObject,
					MessageIds.SetUIMode, new UIModeMessage { UIMode = player.DeviceConfig.UIMode });
			}
		}

		private void DisableAll () {
			if (joystick)
				joystick.SetActive (false);
			if (selectButton)
				selectButton.SetActive (false);
			if (touchpad)
				touchpad.SetActive (false);
			if (twoDTranslationPlane)
				twoDTranslationPlane.SetActive (false);

#if (UNITY_WSA || UNITY_EDITOR)
			if (SelectAndAxesGestures)
				SelectAndAxesGestures.enabled = false;
			if (SelectAxesAndCursorPointerGestures)
				SelectAxesAndCursorPointerGestures.enabled = false;
#endif
		}

		public static bool InMode (UIMode mode) {
			return Instance.CurrentUIMode.Equals (mode);
		}

		public static List<UIMode> GetUIModes () {
			return Enum.GetValues (typeof (UIMode)).Cast<UIMode> ().ToList ();
		}
	}
}