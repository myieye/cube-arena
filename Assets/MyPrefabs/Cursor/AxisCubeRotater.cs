using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.Interaction.Abstract;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
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

		protected override void Start () {
			base.Start ();
		}

		protected override void Update () {
			if (stateManager.IsRotating ()) {
				rotationWaitTime += Time.deltaTime;
			}
			if (HasRotationInput ()) {
				rotationWaitTime = 0;
			}

			base.Update ();
		}

		protected override void Rotate () {
			if (selectedRigidbody != null) {
				var torque = CalculateRotationTorque ();
				selectedRigidbody.AddTorque (torque, ForceMode.VelocityChange);
				CmdApplyTorqueOnNetwork (selectedRigidbody.gameObject, torque.ToServerDirection());
			}
		}

		[Command]
		protected void CmdApplyTorqueOnNetwork (GameObject cube, Vector3 torque) {
			RpcApplyTorqueOnOtherClients (cube, torque);
		}

		[ClientRpc]
		void RpcApplyTorqueOnOtherClients (GameObject cube, Vector3 torque) {
			if (!hasAuthority) {
				cube.GetComponent <Rigidbody> ().AddTorque (
					torque.ToLocalDirection (), ForceMode.VelocityChange);
			}
		}

		protected override bool IsStartingRotate () {
			return stateManager.HasSelection () && HasRotationInput () &&
				!stateManager.IsMoving () && (!CrossPlatformInputManager.GetButton (Buttons.Select) ||
                UIModeManager.InUIMode(UIMode.HMD1_Gaze));
		}

		protected override void StartRotate () {
			SelectCube (stateManager.SelectedCube.Cube);
		}

		private void SelectCube (GameObject cube) {
			selectedRigidbody = cube.GetComponent<Rigidbody> ();
			selectedRigidbody.maxAngularVelocity = Settings.Instance.MaxRotationVelocity;
			rotationWaitTime = 0;
		}

		protected override bool IsEndingRotate () {
			return rotationWaitTime > Settings.Instance.RotationTimeout &&
				selectedRigidbody.angularVelocity.magnitude < Settings.Instance.MinRotationVelocity;
		}

		protected virtual Vector3 CalculateRotationTorque () {
			var x = CrossPlatformInputManager.GetAxis (Axes.Horizontal) * speed;
			var y = CrossPlatformInputManager.GetAxis (Axes.Vertical) * speed;
			var cameraRelativeTorque = Camera.main.transform.TransformDirection (new Vector3 (y, 0, -x));
			cameraRelativeTorque.y = 0;
			return cameraRelativeTorque;
		}

		protected bool HasRotationInput () {
			return CalculateRotationTorque ().magnitude > 0;
		}
	}
}