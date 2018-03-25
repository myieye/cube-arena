using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyPrefabs.Cursor;
using CubeArena.Assets.MyScripts.Interaction;
using UnityEngine;
using UnityEngine.Events;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.UI.Mode
{
	public class UIModeManager : MonoBehaviour {
		
		/*
		Relevant components:
			- Cursor controller

		UI:
			- Joystick
			- Select button
			- Touch pad
		*/

		public GameObject joystick;
		public GameObject selectButton;
		public GameObject touchpad;
		
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

		public UIMode defaultUIMode;
		public CursorController.CursorMode CurrentCursorMode { get; private set; }
		private UIMode currentUIMode;
		public UIMode CurrentUIMode {
			get {
				return currentUIMode;
			}
			set {
				currentUIMode = value;
				ChangeUIMode(currentUIMode);
			}
		}
		private UIMode[] modes = (UIMode[]) Enum.GetValues(typeof(UIMode));

		void Start () {
			CurrentUIMode = defaultUIMode;
		}
		
		void Update () {
			
		}

		public void OnUIModeChanged(int uiMode) {
			CurrentUIMode = modes[uiMode];
		}

		private void ChangeUIMode(UIMode mode) {
			DisableAll();
			switch (mode) {
				case UIMode.Mouse:
					CurrentCursorMode = CursorController.CursorMode.Mouse;
					CrossPlatformInputManager.SwitchActiveInputMethod(
						CrossPlatformInputManager.ActiveInputMethod.Hardware);
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
	}
}