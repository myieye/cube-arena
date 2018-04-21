using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.Logging.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Interaction.Abstract {
	public abstract class AbstractCubeSelecter : NetworkBehaviour {

		protected InteractionStateManager stateManager;

		protected virtual void Start () {
			stateManager = GetComponentInParent<InteractionStateManager> ();
		}

		protected virtual void Update () {
			if (!isLocalPlayer) return;
			if (stateManager.IsSpraying ()) return;

			var selection = CheckStartingNewSelect ();
			var deselecting = CheckEndingSelect (selection);
			if (!selection && !deselecting && !stateManager.HasSelection () && IsPressingSelect ()) {
				Measure.Instance.MadeSelection (SelectionActionType.Miss);
			}
		}

		private GameObject CheckStartingNewSelect () {
			GameObject cube = null;
			if (!stateManager.InState (InteractionState.Disallowed) && IsSelecting (out cube)) {
				stateManager.Select (cube);
			}
			return cube;
		}

		private bool CheckEndingSelect (GameObject justSelected) {
			if (stateManager.HasSelection () && IsDeselecting () && !stateManager.IsSelected (justSelected)) {
				stateManager.Deselect ();
				return true;
			}
			return false;
		}

		protected abstract bool IsSelecting (out GameObject cube);
		protected abstract bool IsDeselecting ();
		protected abstract bool IsPressingSelect ();
	}
}