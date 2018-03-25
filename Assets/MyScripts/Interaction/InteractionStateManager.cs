using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CubeArena.Assets.MyPrefabs.Cubes;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.Logging.Models;

namespace CubeArena.Assets.MyScripts.Interaction
{
	public class InteractionStateManager : MonoBehaviour {

		public InteractionState State { get; private set; }
		public CubeStatePair HoveredCube { get; private set; }
		public CubeStatePair SelectedCube { get; private set; }

		public void Update() {
			if (IsMoving()) {
				Measure.UpdateMove(SelectedCube.Cube);
			} else if (IsRotating()) {
				Measure.UpdateRotation(SelectedCube.Cube);
			}
		}

		public void StartHover(GameObject cube) {
			if (!IsHovered(cube)) {
				EndHover();
				HoveredCube = new CubeStatePair(cube);
			}
			if (!IsSelected(cube)) {
				HoveredCube.StateManager.Hover();
			}
		}

		public void EndHover() {
			if (HoveredCube != null) {
				if (!IsSelected(HoveredCube.Cube)) {
					HoveredCube.StateManager.Unhover();
				}
				HoveredCube = null;
			}
		}

		public void Select(GameObject cube) {
			var reselecting = HasSelection() && !IsSelected(cube);
			if (reselecting) {
				Measure.MadeSelection(SelectionActionType.Reselect);
			}
			Deselect(reselecting);
			if (!reselecting) {
				Measure.MadeSelection(SelectionActionType.Select);
			}
			if (IsHovered(cube)) {
				SelectedCube = HoveredCube;
			} else {
				SelectedCube = new CubeStatePair(cube);
			}
			SelectedCube.StateManager.Select();
			State = InteractionState.Selected;
		}

		public void Deselect(bool reselecting = false) {
			FinishAnyMeasurements();
			if (SelectedCube != null) {
				if (!reselecting) {
					Measure.MadeSelection(SelectionActionType.Deselect);
				}

				SelectedCube.StateManager.Deselect();
				SelectedCube = null;
			}
			State = InteractionState.Idle;
		}

		public void StartMove() {
			FinishAnyMeasurements();
			if (SelectedCube != null && !InState(InteractionState.Disallowed)) {
				State = InteractionState.Moving;
			} else if (HoveredCube != null) {
				Select(HoveredCube.Cube);
				State = InteractionState.Moving;
			}
			Measure.StartMove(SelectedCube.Cube);
		}

		public void EndMove() {
			Measure.EndMove(SelectedCube.Cube);
			State = InteractionState.Selected;
		}

		public void StartRotation() {
			FinishAnyMeasurements();
			if (IsMoving()) {
				if (InState(InteractionState.Disallowed)) {
					Debug.LogError("StartRotation while disallowed");
				} else {
					Debug.LogWarning("StartRotation while Moving");
				}
			}
			if (SelectedCube != null) {
				State = InteractionState.Rotating;
				Measure.StartRotation(SelectedCube.Cube);
				SelectedCube.StateManager.CmdStartRotation();
			}
		}

		public void EndRotation() {
			SelectedCube.StateManager.CmdEndRotation();
			Measure.EndRotation(SelectedCube.Cube);
			State = InteractionState.Selected;
		}

		public void StartDisallow() {
			if (InState(InteractionState.Moving)) {
				SelectedCube.StateManager.Disallow();
				State = InteractionState.Disallowed;
			}
		}

		public void EndDisallow() {
			SelectedCube.StateManager.Reallow();
			State = InteractionState.Moving;
		}

		public bool IsHovering() {
			return HoveredCube != null;
		}

		public bool HasSelection() {
			return SelectedCube != null;
		}

		public bool IsHovered(GameObject cube) {
			return HoveredCube != null && HoveredCube.Cube.Equals(cube);
		}

		public bool IsSelected(GameObject cube) {
			return SelectedCube != null && SelectedCube.Cube.Equals(cube);
		}

		public bool IsMoving() {
			return InStates(InteractionState.Moving, InteractionState.Disallowed);
		}

		public bool InState(InteractionState state) {
			return state.Equals(State);
		}

		public bool InStates(params InteractionState[] states) {
			return states.Contains(State);
		}

		private void FinishAnyMeasurements() {
			if (InState(InteractionState.Moving)) {
				Measure.EndMove(SelectedCube.Cube);
				Debug.LogWarning("FinishAnyMeasurements.EndMove");
			} else if (IsRotating()) {
				Measure.EndRotation(SelectedCube.Cube);
			}
		}

		public bool IsRotating() {
			return InState(InteractionState.Rotating);
		}
	}


	public class CubeStatePair {
		public CubeStatePair(GameObject cube) {
			Cube = cube;
			StateManager = cube.GetComponent<CubeStateManager>();
		}

		public GameObject Cube { get; private set; }
		public CubeStateManager StateManager { get; private set; }
	}

	public enum InteractionState { Idle, Moving, Rotating, Selected, Disallowed }
}