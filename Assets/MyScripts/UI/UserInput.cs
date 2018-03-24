using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
namespace CubeArena.Assets.MyScripts.UI.Mode
{
	public class UserInput : MonoBehaviour {

		private Joystick joystick;
		private Rect guiArea;

		// Use this for initialization
		void Start () {
			joystick = FindObjectOfType<Joystick>();
			var rectTransform = joystick.GetComponent<RectTransform>();
			Vector3[] corners = new Vector3[4];
			rectTransform.GetWorldCorners(corners);
			guiArea = new Rect(corners[1].x, corners[0].y, corners[2].x - corners[1].x, corners[1].y - corners[0].y);
		}
		
		// Update is called once per frame
		void Update () {
			//Debug.Log(Input.mousePosition);
		}

		public bool onGui() {
			var onGui = guiArea.Contains(Input.mousePosition);
			Debug.Log("On Rect: " + onGui);
			return onGui;
		}

		public float GetXAxis() {
			return joystick.Horizontal;
		}

		public float GetYAxis() {
			return joystick.Vertical;
		}
	}
}
 */