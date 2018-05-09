using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.Utils;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Interaction.Abstract {
	public abstract class AbstractSprayToggle : MonoBehaviour {

		private InteractionStateManager _stateManager;
		protected InteractionStateManager StateManager {
			get {
				if (!_stateManager) {
					_stateManager = GameObjectUtil.FindLocalAuthoritativeObject<InteractionStateManager> ();
				}
				return _stateManager;
			}
		}

		public virtual void ToggleState () {
			if (StateManager) {
				if (StateManager.IsSpraying ()) {
					StateManager.EndSpray ();
				} else {
					StateManager.StartSpray ();
				}
			}
		}

		public virtual void ResetToMove () {
			if (StateManager) {
				if (StateManager.IsSpraying ()) {
					StateManager.EndSpray ();
				}
			}
		}
	}
}