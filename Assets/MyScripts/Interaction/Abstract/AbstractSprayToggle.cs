using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Interaction;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Interaction.Abstract {
	public class AbstractSprayToggle : MonoBehaviour {

		private InteractionStateManager _stateManager;
		protected InteractionStateManager StateManager {
			get {
				if (!_stateManager) {
					_stateManager = FindObjectOfType<InteractionStateManager> ();
				}
				return _stateManager;
			}
		}

		public virtual void ToggleState () {
			if (StateManager.IsSpraying ()) {
				StateManager.EndSpray ();
			} else {
				StateManager.StartSpray ();
			}
		}
	}
}