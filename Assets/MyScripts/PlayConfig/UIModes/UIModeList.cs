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
				from mode in Enum.GetValues (typeof (UIMode)).Cast<UIMode> () select new Dropdown.OptionData (mode.GetFriendlyString ())).ToList ();
		}

		public void RefreshSelectedUIMode () {
			dropdown.value = ((int) UIModeManager.Instance.CurrentUIMode) - 1;
		}
	}
}