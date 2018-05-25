using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Logging.DAL.Mock;
using CubeArena.Assets.MyScripts.Logging.DAL.Models;
using CubeArena.Assets.MyScripts.Logging.DAL.SQLite;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Settings;

namespace CubeArena.Assets.MyScripts.Logging.DAL {

	public class DataService {

		private CubeArenaMeasurementsDb db;
		private DataService () {
			if (Settings.Instance.LogMeasurementsToDb) {
				db = new CubeArenaMeasurementsSQLiteDb ();
			} else {
				db = new CubeArenaMeasurementsMockDb ();
			}
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
			if (Settings.Instance.LogMeasurementsToConsole) {
				UnityEngine.Debug.Log (msg);
			}
		}

		public void SaveMove (Move move) {
			Log (SaveToDb (db.InsertMove, move));
		}

		public void SaveRotation (Rotation rotation) {
			Log (SaveToDb (db.InsertRotation, rotation));
		}

		public void SaveSelectionAction (SelectionAction selectionAction) {
			Log (SaveToDb (db.InsertSelectionAction, selectionAction));
		}

		public void SaveSelecion (Selection selection) {
			Log (SaveToDb (db.InsertSelection, selection));
		}

		public void SavePlacement (Placement placement) {
			Log (SaveToDb (db.InsertPlacement, placement));
		}

		public void SaveKill (Kill kill, List<Assist> assists) {
			Log (SaveToDb (db.InsertKill, kill));
			foreach (var assist in assists) {
				Log (SaveToDb (db.InsertAssist, assist));
			}
		}

		public void SaveAreaInteraction (AreaInteraction areaInteraction) {
			Log (SaveToDb (db.InsertAreaInteraction, areaInteraction));
		}

		public void SaveCloudMeasurement (CloudMeasurement cloudMeasurement) {
			Log (SaveToDb (db.InsertCloudMeasurement, cloudMeasurement));
		}

		public void SaveDeviceIfNewModel (Device device) {
			var d = db.GetDeviceByModel (device.Model);
			if (d == null) {
				Log (SaveToDb (db.InsertDevice, device));
			} else {
				device.Id = d.Id;
			}
		}

		private T SaveToDb<T> (Func<T, T> saveFunc, T entity) {
			return saveFunc (entity);
		}

		public int GetNextPlayerId () {
			return db.GetNextPlayerId ();
		}

		public List<int> CreatePlayerRoundIds (List<NetworkPlayer> players, int roundNum) {
			var playerRoundIds = new List<int> ();
			foreach (var player in players) {
				var playerRound = new PlayerRound {
					PlayerId = player.PlayerId,
						RoundNum = roundNum,
						UI = player.DeviceConfig.UIMode,
						DeviceId = player.DeviceConfig.Device.Id
				};
				playerRoundIds.Add (db.InsertPlayerRound (playerRound).Id);
			}
			return playerRoundIds;
		}
	}
}