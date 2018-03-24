using System;
using CubeArena.Assets.MyScripts.Constants;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.Interaction
{
	public class SelectionTouchPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		public void OnPointerDown(PointerEventData data)
		{
			CrossPlatformInputManager.SetButtonDown(Buttons.Select);
		}


		public void OnPointerUp(PointerEventData data)
		{
			CrossPlatformInputManager.SetButtonUp(Buttons.Select);
		}
	}
}