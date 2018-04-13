using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CubeArena.Assets.MyPrefabs.Cubes;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.Logging.Models;
using CubeArena.Assets.MyScripts.Constants;
using CubeArena.Assets.MyScripts.Interaction.Listeners;

namespace CubeArena.Assets.MyScripts.Interaction
{
	public class InteractionStateManager : MonoBehaviour {

		public InteractionState State { get; private set; }
		public CubeStatePair HoveredCube { get; private set; }
		public CubeStatePair SelectedCube { get; private set; }
		private List<OnCubeDeselectedListener> onCubeDeselectedListeners;
		private InteractionState prevState = InteractionState.Idle;


		public void Awake() {
			onCubeDeselectedListeners = new List<OnCubeDeselectedListener>();
		}

		public void Update() {
			if (Settings.Instance.LogInteractionStateChanges && prevState != State) {
				Debug.Log("Entered State: " + State);
				prevState = State;
			}

			if (IsMoving()) {
				Measure.Instance.UpdateMove(SelectedCube.Cube);
			} else if (IsRotating()) {
				Measure.Instance.UpdateRotation(SelectedCube.Cube);
			}
		}

		public void AddOnCubeDeselectedListener(OnCubeDeselectedListener onCubeDeselectedListener) {
			onCubeDeselectedListeners.Add(onCubeDeselectedListener);
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
			Debug.Log("StateManager.Select");
			var reselecting = HasSelection() && !IsSelected(cube);
			if (reselecting) {
				Measure.Instance.MadeSelection(SelectionActionType.Reselect);
			}
			Deselect(reselecting);
			if (!reselecting) {
				Measure.Instance.MadeSelection(SelectionActionType.Select);
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
					Measure.Instance.MadeSelection(SelectionActionType.Deselect);
				}
				SelectedCube.StateManager.Deselect();
				onCubeDeselectedListeners.ForEach(l => l.OnCubeDeselected(SelectedCube.Cube));
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
			Measure.Instance.StartMove(SelectedCube.Cube);
		}

		public void EndMove() {
			Debug.Log("StateManager.EndMove");
			Measure.Instance.EndMove(SelectedCube.Cube);
			State = InteractionState.Selected;
		}

		public void StartRotation() {
			Debug.Log("StateManager.StartRotation");
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
				Measure.Instance.StartRotation(SelectedCube.Cube);
				SelectedCube.StateManager.CmdStartRotation();
			}
		}

		public void EndRotation() {
			Debug.Log("StateManager.EndRotation");
			SelectedCube.StateManager.CmdEndRotation();
			Measure.Instance.EndRotation(SelectedCube.Cube);
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
				Measure.Instance.EndMove(SelectedCube.Cube);
				Debug.LogWarning("FinishAnyMeasurements.EndMove");
			} else if (IsRotating()) {
				Measure.Instance.EndRotation(SelectedCube.Cube);
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