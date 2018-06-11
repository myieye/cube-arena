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

		
		[SerializeField]
		private int touchCount;
		private bool valid;
		private bool dropdownEnabled;

		private bool ToggleActive {
			get {
#if UNITY_EDITOR || UNITY_STANDALONE
				return Input.GetMouseButtonDown (1);
#elif UNITY_ANDROID
				return Input.touchCount >= touchCount;
#elif UNITY_WSA
				return false;
#endif
			}
		}

		public static UIModeList Instance { get; private set; }

		void Awake () {
			valid = true;
			dropdownEnabled = true;

			SetVisible (false);
			Instance = this;
			dropdown = GetComponent<Dropdown> ();
			dropdown.options = (
				from mode in UIModeHelpers.UIModesForCurrentDevice select new Dropdown.OptionData (mode.GetFriendlyString ())).ToList ();
		}

		private void Update () {
			if (valid && ToggleActive) {
				SetEnabled (!dropdownEnabled);
				valid = false;
			} else if (!ToggleActive) {
				valid = true;
			}
		}

		public void RefreshSelectedUIMode () {
			var currMode = UIModeManager.Instance<UIModeManager> ().CurrentUIMode;
			dropdown.value = UIModeHelpers.UIModesForCurrentDevice.ToList ().IndexOf (currMode);
		}

		public void SetEnabled (bool enabled) {
			if (dropdown) {
				dropdown.enabled = enabled;
				dropdownArrow.SetActive (enabled);
				dropdownEnabled = enabled;
			}
		}

		public void SetVisible (bool visible) {
			if (gameObject) {
				gameObject.SetActive (visible);
			}
		}
	}
}