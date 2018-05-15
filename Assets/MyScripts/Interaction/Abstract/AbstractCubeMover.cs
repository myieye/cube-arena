using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Interaction.State;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.Logging.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Interaction.Abstract {
	public abstract class AbstractCubeMover : NetworkBehaviour {

		protected InteractionStateManager stateManager;

		protected virtual void Start () {
			stateManager = GetComponent<InteractionStateManager> ();
		}

		protected virtual void Update () {
			if (!isLocalPlayer) return;
			if (stateManager.IsSpraying ()) return;

			var starting = CheckStartMoving ();
			if (!starting) {
				CheckEndMoving ();
			}
			CheckDisallow ();
		}

		protected virtual void FixedUpdate () {
			if (!isLocalPlayer) return;
			if (stateManager.IsSpraying ()) return;

			if (stateManager.IsMoving ()) {
				Move ();
			}
		}

		bool CheckStartMoving () {
			GameObject cube;
			if (!stateManager.IsMoving () && IsStartingMove (out cube)) {
				StartMove (cube);
				stateManager.StartMove ();
				return true;
			}
			return false;
		}

		void CheckEndMoving () {
			if (stateManager.IsMoving () && IsEndingMove ()) {
				if (!stateManager.InState (InteractionState.Disallowed)) {
					EndMove ();
					stateManager.EndMove ();
				} else {
					Measure.Instance.MadeSelection (SelectionActionType.Disallowed);
				}
			}
		}

		void CheckDisallow () {
			if (stateManager.IsMoving () && !stateManager.InState (InteractionState.Disallowed) && IsDisallowed ()) {
				stateManager.StartDisallow ();
			} else if (stateManager.InState (InteractionState.Disallowed) && !IsDisallowed ()) {
				stateManager.EndDisallow ();
			}
		}

		protected abstract bool IsStartingMove (out GameObject cube);
		protected abstract bool IsEndingMove ();
		protected abstract bool IsDisallowed ();
		protected abstract void Move ();
		protected abstract void StartMove (GameObject cube);
		protected abstract void EndMove ();
	}
}