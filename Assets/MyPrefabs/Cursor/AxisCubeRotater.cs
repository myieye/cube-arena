﻿using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Constants;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.Interaction.Abstract;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyPrefabs.Cursor
{
	public class AxisCubeRotater : AbstractCubeRotater {

		public float speed = 100;
		public float maxSpeed = 3000;
		private Rigidbody selectedRigidbody;
		private float rotationWaitTime;
		private Settings settings;

		protected override void Start() {
			base.Start();
			settings = FindObjectOfType<Settings>();
		}

		protected override void Update() {
			if (stateManager.InState(InteractionState.Rotating)) {
				rotationWaitTime += Time.deltaTime;
			}
			if (HasRotationInput()) {
				rotationWaitTime = 0;
			}
			
			base.Update();
		}

		protected override void Rotate() {
			if (selectedRigidbody != null) {
				selectedRigidbody.AddTorque(CalculateRotationTorque(), ForceMode.VelocityChange);
			}
		}

		protected override bool IsStartingRotate() {
			return stateManager.HasSelection() && HasRotationInput() &&
				!stateManager.IsMoving() && !CrossPlatformInputManager.GetButton(Buttons.Select);
		}

		protected override void StartRotate() {
			SelectCube(stateManager.SelectedCube.Cube);
		}

        private void SelectCube(GameObject cube) {
			selectedRigidbody = cube.GetComponent<Rigidbody>();
			selectedRigidbody.maxAngularVelocity = settings.MaxRotationVelocity;
			rotationWaitTime = 0;
        }

        protected override bool IsEndingRotate() {
			return
				rotationWaitTime > settings.RotationTimeout &&
				selectedRigidbody.angularVelocity.magnitude < settings.MinRotationVelocity;
		}

		protected virtual Vector3 CalculateRotationTorque() {
			var x = CrossPlatformInputManager.GetAxis(Axes.Horizontal) * speed;
			var y = CrossPlatformInputManager.GetAxis(Axes.Vertical) * speed;
			var cameraRelativeTorque = Camera.main.transform.TransformDirection(new Vector3(y, 0, -x));
			cameraRelativeTorque.y = 0;
			return cameraRelativeTorque;
		}

		protected bool HasRotationInput() {
			return CalculateRotationTorque().magnitude > 0;
		}
	}
}