using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

namespace CubeArena.Assets.MyScripts.UI.Mode
{
	[RequireComponent(typeof(Dropdown))]
	public class UIModeList : MonoBehaviour {

		private Dropdown dropdown;

		void Start () {
			dropdown = GetComponent<Dropdown>();
			dropdown.options = (
				from mode
					in Enum.GetValues(typeof(UIMode)).Cast<UIMode>()
					select new Dropdown.OptionData(mode.GetFriendlyString())).ToList();
			RefreshSelectedUIMode();
		}

		public void RefreshSelectedUIMode() {
			dropdown.value = ((int) UIModeManager.Instance.CurrentUIMode) - 1;
		}
	}
}