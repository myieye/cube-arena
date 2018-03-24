using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Logging;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Interaction
{
	public abstract class AbstractCubeRotater : NetworkBehaviour {

		protected InteractionStateManager stateManager;
		
		protected virtual void Start () {
			stateManager = GetComponentInParent<InteractionStateManager>();
		}
		
		protected virtual void Update () {
			if (!isLocalPlayer) return;

			CheckStartRotating();
			CheckEndRotating();
		}

		protected virtual void FixedUpdate() {
			if (!isLocalPlayer) return;
			
			if (Rotating()) {
				Rotate();
			}
		}

		void CheckStartRotating() {
			if (!Rotating() && IsStartingRotate()) {
				stateManager.StartRotation();
			}
		}

		void CheckEndRotating() {
			if (Rotating() && IsEndingRotate()) {
				stateManager.EndRotation();
			}
		}

		protected bool Rotating() {
			return stateManager.InState(InteractionState.Rotating);
		}

		protected abstract void Rotate();
		protected abstract bool IsStartingRotate();
		protected abstract bool IsEndingRotate();
	}
}