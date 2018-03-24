﻿using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.Logging.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Interaction
{
	public abstract class AbstractCubeMover : NetworkBehaviour {

		protected InteractionStateManager stateManager;
		
		protected virtual void Start () {
			stateManager = GetComponentInParent<InteractionStateManager>();
		}
		
		protected virtual void Update () {
			if (!isLocalPlayer) return;

			var starting = CheckStartMoving();
			if (!starting) {
				CheckEndMoving();
			}
			CheckDisallow();
		}

		protected virtual void FixedUpdate() {
			if (!isLocalPlayer) return;
			
			if (stateManager.IsMoving()) {
				Move();
			}
		}

		bool CheckStartMoving() {
			if (!stateManager.IsMoving() && IsStartingMove()) {
				stateManager.StartMove();
				return true;
			}
			return false;
		}

		void CheckEndMoving() {
			if (stateManager.IsMoving() && IsEndingMove()) {
				if (!stateManager.InState(InteractionState.Disallowed)) {
					EndMove();
					stateManager.EndMove();
				} else {
					Measure.MadeSelection(SelectionActionType.Disallowed);
				}
			}
		}

		void CheckDisallow() {
			if (stateManager.IsMoving() && !stateManager.InState(InteractionState.Disallowed) && IsDisallowed()) {
				stateManager.StartDisallow();
			} else if (stateManager.InState(InteractionState.Disallowed) && !IsDisallowed()) {
				stateManager.EndDisallow();
			}
		}

		protected abstract bool IsStartingMove();
		protected abstract bool IsEndingMove();
		protected abstract bool IsDisallowed();
		protected abstract void Move();
		protected abstract void EndMove();
	}
}