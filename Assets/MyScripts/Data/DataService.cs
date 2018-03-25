using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Data.Models;
using CubeArena.Assets.MyScripts.Data.SQLite;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Data {

	public class DataService {

		private CubeArenaMeasurementsDb db;
		private DataService () {
			db = new CubeArenaMeasurementsDb ();
		}

		private static DataService instance;
		public static DataService Instance {
			get {
				if (instance == null) {
					instance = new DataService ();
				}
				return instance;
			}
		}

		public void Log (object msg) {
			Debug.Log (msg);
		}

		public void SaveMove (Move move) {
			Log (db.InsertMove (move));
		}

		public void SaveRotation (Rotation rotation) {
			Log (db.InsertRotation (rotation));
		}

		public void SaveSelectionAction (SelectionAction selectionAction) {
			Log (db.InsertSelectionAction (selectionAction));
		}

		public void SaveSelecion (Selection selection) {
			Log (db.InsertSelection (selection));
		}

		public void SavePlacement (Placement placement) {
			Log (db.InsertPlacement (placement));
		}

		public void SaveKill (Kill kill, List<Assist> assists) {
			Log (db.InsertKill (kill));
			foreach (var assist in assists) {
				Log (db.InsertAssist (assist));
			}
		}

		public int GetNextPlayerId () {
			return db.GetNextPlayerId ();
		}

		public void SaveAreaInteraction (AreaInteraction areaInteraction) {
			Log (db.InsertAreaInteraction(areaInteraction));
		}

		public List<int> CreatePlayerRoundIds (IEnumerable<int> playerIds, int roundNum) {
			var playerRoundIds = new List<int> ();
			foreach (var playerId in playerIds) {
				var playerRound = new PlayerRound {
					PlayerId = playerId,
						RoundNum = roundNum,
						// TODO - add ui
				};
				playerRoundIds.Add (db.InsertPlayerRound (playerRound).Id);
			}
			return playerRoundIds;
		}
	}
}