using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyPrefabs.Cubes
{
	public enum CubeState { None, Drag, Hover, Select, Disallow }

	public class CubeStateManager : NetworkBehaviour {

		public CubeState State { get; private set; }
		private CubeColourer colourer;
		public bool IsRotating { get; private set; }

		void Start() {
			State = CubeState.None;
			colourer = GetComponentInParent<CubeColourer>();
		}

		public void Hover() {
			if (!InStates(CubeState.Drag, CubeState.Select, CubeState.Disallow)) {
				State = CubeState.Hover;
				ColourCube();
			}
		}

		public void Unhover() {
			if (!InStates(CubeState.Drag, CubeState.Select, CubeState.Disallow)) {
				State = CubeState.None;
				ColourCube();
			}
		}

		public void StartDrag() {
			State = CubeState.Drag;
			ColourCube();
		}

		public void EndDrag() {
			State = CubeState.Select;
			ColourCube();
		}

		public void Select() {
			State = CubeState.Select;
			ColourCube();
		}

		public void Deselect() {
			State = CubeState.None;
			ColourCube();
		}

		public void Disallow() {
			State = CubeState.Disallow;
			ColourCube();
		}

		public void Reallow() {
			State = CubeState.Drag;
			ColourCube();
		}

		public bool InState(CubeState cubeState) {
			return cubeState.Equals(State);
		}

		public bool InStates(params CubeState[] cubeStates) {
			return cubeStates.Contains(State);
		}

		private void ColourCube() {
			switch(State) {
				case CubeState.Hover:
					colourer.Hover();
					break;
				case CubeState.Drag:
					colourer.Select();
					break;
				case CubeState.Select:
					colourer.Select();
					break;
				case CubeState.Disallow:
					colourer.MarkDisallowed();
					break;
				default:
					colourer.Unhighlight();
					break;
			}
		}

		/*
			For logging on the server!
		 */
		[Command]
		public void CmdStartRotation() {
			IsRotating = true;
		}

		[Command]
		public void CmdEndRotation() {
			IsRotating = false;
		}
	}
}