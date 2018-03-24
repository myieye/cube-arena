using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Logging.Models;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace CubeArena.Assets.MyScripts.Logging {
	public class LocalLogger : NetworkBehaviour {

		private ServerLogger serverLogger;

		void Start () {
			if (isServer) {
				serverLogger = ServerLogger.Logger;
			}
		}

		[Command]
		public void CmdLog (string msg) {
			serverLogger.Log (msg);
		}

		[Command]
		public void CmdLogMove (Move move) {
			serverLogger.LogMove (move);
		}

		[Command]
		public void CmdLogRotation (Rotation rotation) {
			serverLogger.LogRotation (rotation);
		}

		[Command]
		public void CmdLogSelectionAction (SelectionAction selectionAction) {
			serverLogger.LogSelectionAction (selectionAction);
		}

		[Command]
		public void CmdLogSelection (Selection selection) {
			serverLogger.LogSelecion (selection);
		}

		[Command]
		public void CmdLogPlacement (Placement placement) {
			serverLogger.LogPlacement (placement);
		}

		[Command]
		public void CmdLogKill (Kill kill, Assist[] assists) {
			serverLogger.LogKill(kill, new List<Assist>(assists));
		}
	}
}