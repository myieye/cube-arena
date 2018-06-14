using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.Interaction.Abstract;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Settings;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyPrefabs.Cursor {
	public class AxisCubeRotater : AbstractCubeRotater {

		public float speed = 100;
		public float maxSpeed = 3000;
		private Rigidbody selectedRigidbody;
		private float rotationWaitTime;

		protected Vector3 savedTorque;

		protected override void Start () {
			base.Start ();
		}

		protected override void Update () {
			if (stateManager.IsRotating ()) {
				rotationWaitTime += Time.deltaTime;
			}

			base.Update ();
		}

		protected override void Rotate () {
			if (selectedRigidbody != null && HasSavedTorque ()) {
				rotationWaitTime = 0;
				var torque = savedTorque; // CalculateRotationTorque ();
				selectedRigidbody.AddTorque (torque, ForceMode.VelocityChange);
				CmdApplyTorqueOnNetwork (selectedRigidbody.gameObject, torque.ToServerDirection ());
			}
		}

		[Command]
		protected void CmdApplyTorqueOnNetwork (GameObject cube, Vector3 torque) {
			RpcApplyTorqueOnOtherClients (cube, torque);
		}

		[ClientRpc]
		void RpcApplyTorqueOnOtherClients (GameObject cube, Vector3 torque) {
			if (!hasAuthority && selectedRigidbody) {
				selectedRigidbody.AddTorque (torque.ToLocalDirection (), ForceMode.VelocityChange);
			}
		}

		protected override bool IsStartingRotate () {
			return stateManager.HasSelection () && HasRotationInput () &&
				!stateManager.IsMoving () && (!CrossPlatformInputManager.GetButton (Buttons.Select) ||
					UIModeManager.InUIMode (UIMode.HMD1_Gaze));
		}

		protected override void StartRotate () {
			SelectCubeOnNetwork (stateManager.SelectedCube.Cube);
		}

		protected void SelectCubeOnNetwork (GameObject cube) {
			SelectCube (cube);
			CmdSelectCubeOnNetwork (cube);
		}

		[Command]
		protected void CmdSelectCubeOnNetwork (GameObject cube) {
			RpcSelectCubeOnOtherClients (cube);
		}

		[ClientRpc]
		void RpcSelectCubeOnOtherClients (GameObject cube) {
			if (!hasAuthority) {
				SelectCube (cube);
			}
		}

		private void SelectCube (GameObject cube) {
			if (cube) {
				selectedRigidbody = cube.GetComponent<Rigidbody> ();
				selectedRigidbody.maxAngularVelocity = Settings.Instance.MaxRotationVelocity;
				SetCubeNetworkTransformEnabled (cube, false);
				rotationWaitTime = 0;
			}
		}

		protected override void EndRotate (bool immediate) {
			if (stateManager.SelectedCube != null) {
				CmdDeselectCubeOnNetwork (stateManager.SelectedCube.Cube, immediate);
			}
		}

		[Command]
		private void CmdDeselectCubeOnNetwork (GameObject cube, bool immediate) {
			RpcDeselectCubeOnClients (cube, immediate);
		}

		[ClientRpc]
		void RpcDeselectCubeOnClients (GameObject cube, bool immediate) {
			selectedRigidbody = null;

			if (cube) {
				if (immediate) {
					SetCubeNetworkTransformEnabled (cube, true);
				} else {
					var savedCube = cube;
					StartCoroutine (DelayUtil.Do (1.5f, () => {
						if (savedCube && (!selectedRigidbody || savedCube != selectedRigidbody.gameObject)) {
							SetCubeNetworkTransformEnabled (savedCube, true);
						}
					}));
				}
			}
		}

		protected override bool IsEndingRotate () {
			return (!selectedRigidbody) ||
				(rotationWaitTime > Settings.Instance.RotationTimeout &&
					selectedRigidbody.angularVelocity.magnitude < Settings.Instance.MinRotationVelocity);
		}

		protected override Vector3 CalculateRotationTorque () {
			var x = CrossPlatformInputManager.GetAxis (Axes.Horizontal) * speed;
			var y = CrossPlatformInputManager.GetAxis (Axes.Vertical) * speed;
			var cameraRelativeTorque = Camera.main.transform.TransformDirection (new Vector3 (y, 0, -x));
			cameraRelativeTorque.y = 0;
			savedTorque = cameraRelativeTorque;
			return cameraRelativeTorque;
		}

		protected bool HasRotationInput () {
			return CalculateRotationTorque ().magnitude > 0;
		}

		private bool HasSavedTorque () {
			return savedTorque.magnitude > 0;
		}

		private void SetCubeNetworkTransformEnabled (GameObject cube, bool enabled) {
			var networkTransform = cube.GetComponent<RelativeNetworkTransform> ();
			if (enabled) {
				networkTransform.ResetTarget ();
			}
			networkTransform.enabled = enabled;
		}
	}
}