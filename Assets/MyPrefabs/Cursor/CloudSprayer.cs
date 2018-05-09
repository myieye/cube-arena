using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
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
		private CursorController cursorCtrl;
		private PlayerId playerId;

		void Start () {
			progressBar = FindObjectOfType<ProgressBarBehaviour> ();
			stateManager = GetComponent<InteractionStateManager> ();
			cursorCtrl = GetComponent<CursorController> ();
			playerId = GetComponent<PlayerId> ();
			Reset ();
		}

		public void Reset () {
			lastSpray = DateTime.Now.AddMilliseconds (-cooldown);
			CurrAmount = capacity;
		}

		void Update () {
			if (!isLocalPlayer) return;

			CurrAmount = Mathf.Min (CurrAmount + (rechargeSpeed * Time.deltaTime), capacity);

			if (stateManager.IsSpraying () && Spraying ()) {
				var pos = cursorCtrl.Position;
				if (pos.HasValue) {
					Spray (pos.Value);
				}
			}
		}

		private void Spray (Vector3 position) {
			lastSpray = DateTime.Now;
			CurrAmount -= Cost;
			CmdSpray (position);
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
			var spray = Instantiate (sprayPrefab);
			spray.transform.rotation = UnityEngine.Random.rotation;
			spray.transform.position = position;
			var rend = spray.GetComponent<MeshRenderer> ();
			var playerColour = PlayerManager.Instance.GetPlayerColor (playerId);
			playerColour.a = rend.material.color.a;
			spray.GetComponent<Colourer> ().color = playerColour;
			return spray;
		}
	}
}