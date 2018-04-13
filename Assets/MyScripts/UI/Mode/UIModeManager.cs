using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyPrefabs.Cursor;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.Interaction.HMD;
using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.UI.Mode
{
	public class UIModeManager : MonoBehaviour {

		public GameObject joystick;
		public GameObject selectButton;
		public GameObject touchpad;
		public RaycastCubeMover RaycastCubeMover {
			get {
				return FindObjectOfType<RaycastCubeMover>();
			}
		}
		public GestureCubeMover GestureCubeMover {
			get {
				return FindObjectOfType<GestureCubeMover>();
			}
		}
		#if (UNITY_WSA || UNITY_EDITOR)
		public SelectAndAxesGestures SelectAndAxesGestures {
			get {
				return FindObjectOfType<SelectAndAxesGestures>();
			}
		}
		public SelectAxesAndCursorPointerGestures SelectAxesAndCursorPointerGestures {
			get {
				return FindObjectOfType<SelectAxesAndCursorPointerGestures>();
			}
		}
		#endif
		public CursorController.CursorMode CurrentCursorMode { get; private set; }
		public UIMode CurrentUIMode { get; private set; }
		public static UIModeManager Instance { get; private set; }

		[SerializeField]
		private UIMode defaultUIMode;
		private UIMode[] modes = (UIMode[]) Enum.GetValues(typeof(UIMode));


		void Awake() {
			if (Instance) {
				Destroy(this);
				return;
			}
			
			Instance = this;
			SetUIMode(defaultUIMode);
		}

		public void OnUIModeChanged(int uiMode) {
			SetUIMode(modes[uiMode]);
		}

		private void SetUIMode(UIMode mode) {
			CurrentUIMode = mode;
			DisableAll();
			switch (mode) {
				case UIMode.Mouse:
					CurrentCursorMode = CursorController.CursorMode.Mouse;
					CrossPlatformInputManager.SwitchActiveInputMethod(
						CrossPlatformInputManager.ActiveInputMethod.Hardware);
					//RaycastCubeMover.enabled = false;
					//GestureCubeMover.enabled = true;
					break;
				case UIMode.HHD1_Camera:
					joystick.SetActive(true);
					selectButton.SetActive(true);
					CurrentCursorMode = CursorController.CursorMode.Camera;
					CrossPlatformInputManager.SwitchActiveInputMethod(
						CrossPlatformInputManager.ActiveInputMethod.Touch);
					break;
				case UIMode.HHD2_TouchAndDrag:
					joystick.SetActive(true);
					touchpad.SetActive(true);
					CurrentCursorMode = CursorController.CursorMode.Touch;
					CrossPlatformInputManager.SwitchActiveInputMethod(
						CrossPlatformInputManager.ActiveInputMethod.Touch);
					break;
				case UIMode.HHD3_Gestures:
					touchpad.SetActive(true);
					CurrentCursorMode = CursorController.CursorMode.Touch;
					CrossPlatformInputManager.SwitchActiveInputMethod(
						CrossPlatformInputManager.ActiveInputMethod.Touch);
					break;
				#if (UNITY_WSA || UNITY_EDITOR)
				case UIMode.HMD4_GazeAndClicker:
					SelectAndAxesGestures.enabled = true;
					CurrentCursorMode = CursorController.CursorMode.Camera;
					// TODO ... wrong input method?
					CrossPlatformInputManager.SwitchActiveInputMethod(
						CrossPlatformInputManager.ActiveInputMethod.Touch);
					break;
				case UIMode.HMD5_Gaze__AirTap_Drag_And_Clicker_Rotate:
					SelectAxesAndCursorPointerGestures.enabled = true;
					CurrentCursorMode = CursorController.CursorMode.Pointer;
					// TODO ... wrong input method?
					CrossPlatformInputManager.SwitchActiveInputMethod(
						CrossPlatformInputManager.ActiveInputMethod.Touch);
					break;
				#endif
			}
		}

		private void DisableAll() {
			if (joystick)
				joystick.SetActive(false);
			if (selectButton)
				selectButton.SetActive(false);
			if (touchpad)
				touchpad.SetActive(false);
				
			#if (UNITY_WSA || UNITY_EDITOR)
			if (SelectAndAxesGestures)
				SelectAndAxesGestures.enabled = false;
			if (SelectAxesAndCursorPointerGestures)
				SelectAxesAndCursorPointerGestures.enabled = false;
			#endif
		}

		public static bool InMode(UIMode mode) {
			return Instance.CurrentUIMode.Equals(mode);
		}
	}
}