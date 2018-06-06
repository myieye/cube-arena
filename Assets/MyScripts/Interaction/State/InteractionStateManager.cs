using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyPrefabs.Cubes;
using CubeArena.Assets.MyScripts.Interaction.Listeners;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.Logging.Models;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;
using UnityEngine.Assertions;

namespace CubeArena.Assets.MyScripts.Interaction.State {
	public class InteractionStateManager : MonoBehaviour {

		public InteractionState State { get; private set; }
		public CubeStatePair HoveredCube { get; private set; }
		public CubeStatePair SelectedCube { get; private set; }
		private List<OnCubeDeselectedListener> onCubeDeselectedListeners;
		private InteractionState prevState = InteractionState.Idle;
		private bool rotationIsLocked = false;
		public bool MovingDisabled { get; set; }

		public void Awake () {
			onCubeDeselectedListeners = new List<OnCubeDeselectedListener> ();
		}

		public void Update () {
			if (Settings.Instance.LogInteractionStateChanges && prevState != State) {
				Debug.Log ("Interaction State: " + State);
				prevState = State;
			}

			if (IsMoving ()) {
				Measure.LocalInstance.UpdateMove (SelectedCube.Cube);
			} else if (IsRotating ()) {
				Measure.LocalInstance.UpdateRotation (SelectedCube.Cube);
			} else if (State.IsSelectionState () && !HasSelection ()) {
				State = InteractionState.Idle;
			}
		}

		public void AddOnCubeDeselectedListener (OnCubeDeselectedListener onCubeDeselectedListener) {
			onCubeDeselectedListeners.Add (onCubeDeselectedListener);
		}

		public void StartHover (GameObject cube) {
			if (!IsHovered (cube)) {
				EndHover ();
				HoveredCube = new CubeStatePair (cube);
			}
			if (!IsSelected (cube)) {
				HoveredCube.StateManager.Hover ();
			}
		}

		public void EndHover () {
			if (IsHovering ()) {
				if (!IsSelected (HoveredCube.Cube)) {
					HoveredCube.StateManager.Unhover ();
				}
				HoveredCube = null;
			}
		}

		public void Select (GameObject cube) {
			var reselecting = HasSelection () && !IsSelected (cube);
			if (reselecting) {
				Measure.LocalInstance.MadeSelection (SelectionActionType.Reselect);
			}
			Deselect (reselecting);
			if (!reselecting) {
				Measure.LocalInstance.MadeSelection (SelectionActionType.Select);
			}
			if (IsHovered (cube)) {
				SelectedCube = HoveredCube;
			} else {
				SelectedCube = new CubeStatePair (cube);
			}
			SelectedCube.StateManager.Select ();
			State = InteractionState.Selected;
		}

		public void Deselect (bool reselecting = false) {
			if (rotationIsLocked) return;

			FinishAnyMeasurements ();
			if (HasSelection ()) {
				if (!reselecting) {
					Measure.LocalInstance.MadeSelection (SelectionActionType.Deselect);
				}
				SelectedCube.StateManager.Deselect ();
				onCubeDeselectedListeners.ForEach (l => l.OnCubeDeselected (SelectedCube.Cube));
				SelectedCube = null;
			}
			State = InteractionState.Idle;
		}

		public bool StartMove () {
			if (MovingDisabled) {
				return false;
			}

			FinishAnyMeasurements ();

			// TODO clean up redundant logic
			if (HasSelection () && !InState (InteractionState.Disallowed)) {
				State = InteractionState.Moving;
			} else if (IsHovering ()) {
				Select (HoveredCube.Cube);
				State = InteractionState.Moving;
			}

			SelectedCube.StateManager.StartDrag ();
			Measure.LocalInstance.StartMove (SelectedCube.Cube);

			return true;
		}

		public void EndMove () {
			SelectedCube.StateManager.EndDrag ();
			Measure.LocalInstance.EndMove (SelectedCube.Cube);
			State = InteractionState.Selected;
		}

		public void StartRotation () {
			FinishAnyMeasurements ();
			if (IsMoving ()) {
				if (InState (InteractionState.Disallowed)) {
					Debug.LogError ("StartRotation while disallowed");
				} else {
					Debug.LogWarning ("StartRotation while Moving");
				}
			}
			if (HasSelection ()) {
				State = InteractionState.Rotating;
				Measure.LocalInstance.StartRotation (SelectedCube.Cube);
				SelectedCube.StateManager.CmdStartRotation ();
			}
		}

		public void EndRotation () {
			SelectedCube.StateManager.CmdEndRotation ();
			Measure.LocalInstance.EndRotation (SelectedCube.Cube);
			State = InteractionState.Selected;
		}

		public void LockRotation () {
			rotationIsLocked = true;
		}

		public void UnlockRotation () {
			rotationIsLocked = false;
		}

		public void StartDisallow () {
			if (InState (InteractionState.Moving)) {
				SelectedCube.StateManager.Disallow ();
				State = InteractionState.Disallowed;
			}
		}

		public void EndDisallow () {
			SelectedCube.StateManager.Reallow ();
			State = InteractionState.Moving;
		}

		public void StartSpray () {
			Deselect ();
			State = InteractionState.Spray;
		}

		public void EndSpray () {
			Assert.AreEqual (State, InteractionState.Spray);
			State = InteractionState.Idle;
		}

		public bool IsHovering () {
			return HoveredCube != null && HoveredCube.Cube;
		}

		public bool HasSelection () {
			return SelectedCube != null && SelectedCube.Cube && State.IsSelectionState ();
		}

		public bool IsHovered (GameObject cube) {
			return IsHovering () && HoveredCube.Cube.Equals (cube);
		}

		public bool IsSelected (GameObject cube) {
			return HasSelection () && SelectedCube.Cube.Equals (cube);
		}

		public bool IsMoving () {
			return InStates (InteractionState.Moving, InteractionState.Disallowed);
		}

		public bool IsRotating () {
			return InState (InteractionState.Rotating);
		}

		public bool IsSpraying () {
			return InState (InteractionState.Spray);
		}

		public bool InState (InteractionState state) {
			return state.Equals (State) &&
				(!state.IsSelectionState () || HasSelection ());
		}

		public bool InStates (params InteractionState[] states) {
			return states.Any (InState);
		}

		private void FinishAnyMeasurements () {
			if (InState (InteractionState.Moving)) {
				Measure.LocalInstance.EndMove (SelectedCube.Cube);
				Debug.LogWarning ("FinishAnyMeasurements.EndMove");
			} else if (IsRotating ()) {
				Measure.LocalInstance.EndRotation (SelectedCube.Cube);
			}
		}
	}
}