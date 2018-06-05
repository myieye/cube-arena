using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Logging.Survey;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyPrefabs.Player {
	public class ServerBridge : NetworkBehaviour {

		public static ServerBridge LocalInstance { get; private set; }

		public override void OnStartAuthority () {
			LocalInstance = this;
		}

		[Command]
		public void CmdOnSurveyComplete (int playerId) {
			FindObjectOfType <Surveyer> ().OnClientCompletedSurvey (playerId);
		}
	}
}