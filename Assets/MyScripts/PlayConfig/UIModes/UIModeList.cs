using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CubeArena.Assets.MyScripts.PlayConfig.UIModes {
	[RequireComponent (typeof (Dropdown))]
	public class UIModeList : MonoBehaviour {

		[SerializeField]
		private Dropdown dropdown;
		[SerializeField]
		private GameObject dropdownArrow;

		public static UIModeList Instance { get; private set; }

		void Awake () {
			SetVisible (false);
			Instance = this;
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
				dropdown.enabled = enabled;
				dropdownArrow.SetActive (enabled);
			}
		}

		public void SetVisible (bool visible) {
			if (gameObject) {
				Debug.Log ("Active: " + visible);
				gameObject.SetActive (visible);
			}
		}
	}
}