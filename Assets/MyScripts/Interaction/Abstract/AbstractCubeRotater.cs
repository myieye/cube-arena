﻿using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Interaction.Listeners;
using CubeArena.Assets.MyScripts.Interaction.State;
using CubeArena.Assets.MyScripts.Logging;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Interaction.Abstract {
	public abstract class AbstractCubeRotater : NetworkBehaviour, OnCubeDeselectedListener {

		protected InteractionStateManager stateManager;
		private bool isRotating;

		protected virtual void Start () {
			stateManager = GetComponent<InteractionStateManager> ();
		}

		public override void OnStartAuthority () {
			base.OnStartAuthority ();
			stateManager = GetComponent<InteractionStateManager> ();
			stateManager.AddOnCubeDeselectedListener (this);
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
				isRotating = true;
			}
		}

		void CheckEndRotating () {
			if (isRotating && (!Rotating () || IsEndingRotate ())) {
				EndRotate (!Rotating ());
				isRotating = false;

				if (Rotating ()) {
					stateManager.EndRotation ();
				}
			}
		}

		protected bool Rotating () {
			return stateManager.IsRotating ();
		}

		public void OnCubeDeselected (GameObject cube) {
			if (isRotating) {
				isRotating = false;
				EndRotate (true);
			}
		}

		protected abstract void Rotate ();
		protected abstract bool IsStartingRotate ();
		protected abstract bool IsEndingRotate ();
		protected abstract void StartRotate ();
		protected abstract void EndRotate (bool immediate);
	}
}