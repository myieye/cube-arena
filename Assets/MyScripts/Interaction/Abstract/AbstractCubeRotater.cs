﻿using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Interaction.State;
using CubeArena.Assets.MyScripts.Logging;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Interaction.Abstract {
	public abstract class AbstractCubeRotater : NetworkBehaviour {

		protected InteractionStateManager stateManager;

		protected virtual void Start () {
			stateManager = GetComponent<InteractionStateManager> ();
		}

		protected virtual void Update () {
			if (!hasAuthority) return;
			if (stateManager.IsSpraying ()) return;

			CheckStartRotating ();
			CheckEndRotating ();
		}

		protected virtual void FixedUpdate () {
			if (!hasAuthority) return;
			if (stateManager.IsSpraying ()) return;

			if (Rotating ()) {
				Rotate ();
			}
		}

		void CheckStartRotating () {
			if (!Rotating () && IsStartingRotate ()) {
				StartRotate ();
				stateManager.StartRotation ();
			}
		}

		void CheckEndRotating () {
			if (Rotating () && IsEndingRotate ()) {
				stateManager.EndRotation ();
			}
		}

		protected bool Rotating () {
			return stateManager.IsRotating ();
		}

		protected abstract void Rotate ();
		protected abstract bool IsStartingRotate ();
		protected abstract bool IsEndingRotate ();
		protected abstract void StartRotate ();
	}
}