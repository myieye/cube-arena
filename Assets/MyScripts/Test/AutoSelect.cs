using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Constants;
using CubeArena.Assets.MyScripts.Interaction;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyScripts.Test
{
	public class AutoSelect : MonoBehaviour {
		public GameObject selection;
		private InteractionStateManager stateManager;

		void Start () {
			stateManager = GetComponent<InteractionStateManager>();
		}
		
		// Update is called once per frame
		void Update () {
			if (CrossPlatformInputManager.GetButtonDown(Buttons.Select)) {
				if (selection != null)
					stateManager.Select(selection);
				else {
					stateManager.Select(GameObject.Find("Cube(Clone)"));	
				}
			}
		}
	}
}