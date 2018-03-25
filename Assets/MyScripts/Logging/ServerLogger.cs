using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Data;
using CubeArena.Assets.MyScripts.Data.Models;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace CubeArena.Assets.MyScripts.Logging {
	public class ServerLogger : NetworkBehaviour {

		private DataService dataService;

		void Start () {
			if (isServer) {
				dataService = DataService.Instance;
			}
		}

		[Command]
		public void CmdLog (string msg) {
			dataService.Log (msg);
		}

		[Command]
		public void CmdLogMove (Move move) {
			dataService.SaveMove (move);
		}

		[Command]
		public void CmdLogRotation (Rotation rotation) {
			dataService.SaveRotation (rotation);
		}

		[Command]
		public void CmdLogSelectionAction (SelectionAction selectionAction) {
			dataService.SaveSelectionAction (selectionAction);
		}

		[Command]
		public void CmdLogSelection (Selection selection) {
			dataService.SaveSelecion (selection);
		}

		[Command]
		public void CmdLogPlacement (Placement placement) {
			dataService.SavePlacement (placement);
		}

		[Command]
		public void CmdLogKill (Kill kill, Assist[] assists) {
			dataService.SaveKill (kill, new List<Assist> (assists));
		}

		[Command]
		public void CmdLogAreaInteraction (AreaInteraction areaInteraction) {
			dataService.SaveAreaInteraction (areaInteraction);
		}
	}
}