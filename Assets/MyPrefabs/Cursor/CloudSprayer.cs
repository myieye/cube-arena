using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyPrefabs.Cloud;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.Interaction.State;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Colors;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using ProgressBar;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyPrefabs.Cursor {
	public class CloudSprayer : NetworkBehaviour {

		private ProgressBarBehaviour progressBar;
		[SerializeField]
		private GameObject sprayPrefab;
		[SerializeField]
		private float capacity;
		[SerializeField]
		private float rechargeSpeed;
		[SerializeField]
		private float cooldown;
		private const float Cost = 1;
		private float _currAmount;
		private float CurrAmount {
			get {
				return _currAmount;
			}
			set {
				_currAmount = value;
				progressBar.Value = ((_currAmount / capacity) * 100);
			}
		}
		private DateTime lastSpray;

		private InteractionStateManager stateManager;
		private EnabledComponent<CursorController> cursor;

		private PlayerId playerId;

		void Start () {
			progressBar = FindObjectOfType<ProgressBarBehaviour> ();
			stateManager = GetComponent<InteractionStateManager> ();
			cursor = new EnabledComponent<CursorController> (gameObject);
			playerId = GetComponent<PlayerId> ();
			Reset ();
		}

		public void Reset () {
			if (!hasAuthority) return;

			lastSpray = DateTime.Now.AddMilliseconds (-cooldown);
			CurrAmount = capacity;
		}

		void Update () {
			if (!hasAuthority) return;

			CurrAmount = Mathf.Min (CurrAmount + (rechargeSpeed * Time.deltaTime), capacity);

			if (stateManager.IsSpraying () && Spraying ()) {
				if (cursor.Get.IsActive) {
					Spray (transform.position);
				}
			}
		}

		private void Spray (Vector3 position) {
			lastSpray = DateTime.Now;
			CurrAmount -= Cost;
			CmdSpray (position.ToServer ());
		}

		private bool Spraying () {
			return (DateTime.Now - lastSpray).TotalMilliseconds >= cooldown &&
				CurrAmount >= Cost &&
				CrossPlatformInputManager.GetButton (Buttons.Select);
		}

		[Command]
		private void CmdSpray (Vector3 position) {
			var spray = GenerateSpray (position);
			NetworkServer.Spawn (spray);
		}

		private GameObject GenerateSpray (Vector3 position) {
			// Prepare colour
			var playerColour = PlayerManager.Instance.GetPlayerColor (playerId);
			playerColour.a = sprayPrefab.GetComponent<MeshRenderer> ().sharedMaterial.color.a;

			// Instantiate
			var spray = Instantiate (sprayPrefab);
			spray.transform.rotation = UnityEngine.Random.rotation;
			spray.transform.position = position;
			spray.GetComponent<Colourer> ().color = playerColour;
			spray.GetComponent<CloudEffectivenessMeasurer> ().PlayerMeasuerer =
				PlayerManager.Instance.GetPlayerMeasurer (GetComponent <PlayerId> ());

			return spray;
		}
	}
}