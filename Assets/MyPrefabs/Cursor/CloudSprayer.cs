using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Interaction;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

namespace CubeArena.Assets.MyPrefabs.Cursor {
	public class CloudSprayer : NetworkBehaviour {

		[SerializeField]
		private GameObject sprayPrefab;
		[SerializeField]
		private float capacity;
		[SerializeField]
		private float rechargeSpeed;
		[SerializeField]
		private float cooldown;
		private const float Cost = 1;
		private float currAmount;
		private DateTime lastSpray;

		private InteractionStateManager stateManager;
		private CursorController cursorCtrl;
		private PlayerId playerId;

		void Start () {
			stateManager = GetComponent<InteractionStateManager> ();
			cursorCtrl = GetComponent<CursorController> ();
			playerId = GetComponent<PlayerId> ();
			lastSpray = DateTime.Now.AddMilliseconds (-cooldown);
			currAmount = capacity;
		}

		void Update () {
			currAmount = Mathf.Min (currAmount + (rechargeSpeed * Time.deltaTime), capacity);

			if (stateManager.IsSpraying () && Spraying ()) {
				var pos = cursorCtrl.Position;
				if (pos.HasValue) {
					CmdSpray (pos.Value);
				}
			}
		}

		private bool Spraying () {
			return (DateTime.Now - lastSpray).TotalMilliseconds >= cooldown &&
				currAmount >= Cost &&
				CrossPlatformInputManager.GetButton (Buttons.Select);
		}

		[Command]
		private void CmdSpray (Vector3 position) {
			lastSpray = DateTime.Now;
			currAmount -= Cost;
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
			rend.material.SetColor ("_Color", playerColour);
			return spray;
		}
	}
}