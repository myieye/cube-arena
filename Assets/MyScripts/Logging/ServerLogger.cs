using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Logging.Models;
using CubeArena.Assets.MyScripts.Logging.SQLite;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Logging {
	public class ServerLogger {

		private CubeArenaMeasurementsDb db;
		private ServerLogger () {
			db = new CubeArenaMeasurementsDb ();
		}

		private static ServerLogger instance;
		public static ServerLogger Logger {
			get {
				if (instance == null) {
					instance = new ServerLogger ();
				}
				return instance;
			}
		}

		public void Log (object msg) {
			Debug.Log (msg);
		}

		public void LogMove (Move move) {
			Log (db.InsertMove (move));
		}

		public void LogRotation (Rotation rotation) {
			Log (db.InsertRotation (rotation));
		}

		public void LogKill (Kill kill) {
			Log (db.InsertKill (kill));
		}

		public void LogSelectionAction (SelectionAction selectionAction) {
			Log (db.InsertSelectionAction (selectionAction));
		}

		public void LogSelecion (Selection selection) {
			Log (db.InsertSelection (selection));
		}

		public void LogPlacement (Placement placement) {
			Log (db.InsertPlacement (placement));
		}

		public void LogKill (Kill kill, List<Assist> assists) {
			Log (db.InsertKill(kill));
			foreach (var assist in assists) {
				Log (db.InsertAssist(assist));
			}
		}
	}
}