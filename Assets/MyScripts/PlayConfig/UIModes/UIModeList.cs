using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CubeArena.Assets.MyScripts.PlayConfig.UIModes {
	[RequireComponent (typeof (Dropdown))]
	public class UIModeList : MonoBehaviour {

		private Dropdown dropdown;

		void Awake () {
			dropdown = GetComponent<Dropdown> ();
			dropdown.options = (
				from mode in UIModeHelpers.UIModesForCurrentDevice select new Dropdown.OptionData (mode.GetFriendlyString ())).ToList ();
		}

		public void RefreshSelectedUIMode () {
			var currMode = UIModeManager.Instance<UIModeManager> ().CurrentUIMode;
			dropdown.value = UIModeHelpers.UIModesForCurrentDevice.ToList ().IndexOf (currMode);
		}

		public void SetEnabled (bool enabled) {
			if (dropdown) {
				dropdown.interactable = enabled;
			}
		}

		public void SetVisible (bool visible) {
			if (gameObject) {
				gameObject.SetActive (visible);
			}
		}
	}
}