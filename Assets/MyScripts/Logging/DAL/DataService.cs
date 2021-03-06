﻿using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Logging.DAL.Mock;
using CubeArena.Assets.MyScripts.Logging.DAL.Models;
using CubeArena.Assets.MyScripts.Logging.DAL.Models.Answers;
using CubeArena.Assets.MyScripts.Logging.DAL.SQLite;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using NetworkPlayer = CubeArena.Assets.MyScripts.PlayConfig.Players.NetworkPlayer;
using System.Linq;
using CubeArena.Assets.MyScripts.Logging.DAL.Models.Counters;
using CubeArena.Assets.MyScripts.PlayConfig.Rounds;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Logging.DAL {

	public class DataService {

		private CubeArenaMeasurementsDb db;

		private DataService () { }

		public void SetDbVersion (DatabaseVersion dbVersion) {
			if (Settings.Instance.LogDbVersion) {
				Debug.LogWarning ("----- Using DB: " + dbVersion + " -----");
			}

			var dbName = string.Format ("{0}_{1}", Database.CubeArenaMeasurementsDatabase, dbVersion);

			CloseDb ();

			switch (dbVersion) {
				case DatabaseVersion.Mock:
					db = new CubeArenaMeasurementsMockDb ();
					break;
				case DatabaseVersion.Release:
					db = new CubeArenaMeasurementsSQLiteDb (dbName, false, false);
					break;
				case DatabaseVersion.Debug:
					db = new CubeArenaMeasurementsSQLiteDb (dbName,
						Settings.Instance.ResetDebugDbOnStart, Settings.Instance.LogMeasurementsToConsole);
					break;

			}
		}

		public void OnDestroy () {
			CloseDb ();
		}

		public void CloseDb () {
			if (db != null) {
				db.OnDestroy ();
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
				assist.KillId = kill.Id;
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

		public void SaveRatingAnswer (RatingAnswer answer) {
			RatingAnswer a = db.Find<RatingAnswer> (ra =>
				ra.PlayerRoundId == answer.PlayerRoundId && ra.RatingId == answer.RatingId);

			if (a != null) {
				answer.Id = a.Id;
			}
			Log (db.SaveRatingAnswer (answer));
		}

		public void SaveDeviceRoundConfigs (List<List<DeviceConfig>> deviceRoundConfigs) {

		}

		public void SaveWeightAnswer (WeightAnswer answer) {
			WeightAnswer a = db.Find<WeightAnswer> (wa =>
				wa.PlayerRoundId == answer.PlayerRoundId && wa.WeightId == answer.WeightId);

			if (a != null) {
				answer.Id = a.Id;
			}
			Log (db.SaveWeightAnswer (answer));
		}

		private T SaveToDb<T> (Func<T, T> saveFunc, T entity) {
			return saveFunc (entity);
		}

		public int GetNextPlayerId () {
			return db.GetNextId<PlayerCounter> ();
		}

		/*
				public List<int> CreatePlayerRoundIds (List<NetworkPlayer> players, int gameConfigId, int roundNum) {
					var playerRoundIds = new List<int> ();
					foreach (var player in players) {
						var playerRound = new PlayerRound {
							GameConfigId = gameConfigId,
								PlayerId = player.PlayerId,
								RoundNum = roundNum,
								UI = player.DeviceConfig.UIMode,
								DeviceId = player.DeviceConfig.Device.Id
						};
						playerRoundIds.Add (db.InsertPlayerRound (playerRound).Id);
					}
					return playerRoundIds;
				} */

		public List<List<int>> CreatePlayerRounds (List<NetworkPlayer> players, List<List<DeviceConfig>> deviceRoundConfigs) {

			var gameConfig = new GameConfig {
				NumberOfPlayers = players.Count,
					NumberOfRounds = deviceRoundConfigs.Count
			};
			int gameConfigId = db.InsertGameConfig (gameConfig).Id;

			var gamePlayerRoundIds = new List<List<int>> ();

			for (int r = 0; r < deviceRoundConfigs.Count; r++) {

				int roundNum = r + 1;
				var rConfig = deviceRoundConfigs[r];

				var playerRoundIds = new List<int> ();

				for (int p = 0; p < rConfig.Count; p++) {
					var player = players[p];
					var devConfig = rConfig[p];

					var playerRound = new PlayerRound {
						GameConfigId = gameConfigId,
							PlayerId = player.PlayerId,
							PlayerNum = player.PlayerNum,
							RoundNum = roundNum,
							UI = devConfig.UIMode,
							DeviceId = devConfig.Device.Id
					};

					playerRoundIds.Add (db.InsertPlayerRound (playerRound).Id);
				}

				gamePlayerRoundIds.Add (playerRoundIds);
			}

			return gamePlayerRoundIds;
		}

		public List<GameConfig> FindGameConfigs (int max = int.MaxValue) {
			return db.FindAll<GameConfig> ()
				.OrderByDescending (gc => gc.Created)
				.Take (max).ToList ();

			/*var counter = db.FindAll<PlayerCounter> ((pc) => true).FirstOrDefault ();
			var gameConfigCount = counter != null ? counter.Count : 0;
			var minGameConfigId = (gameConfigCount - max) + 1;

			var playerRounds = db.FindAll<PlayerRound> (pr => pr.GameConfigId >= minGameConfigId);
			return playerRounds.GroupBy (pr => pr.GameConfigId)
				.OrderByDescending (gameConfig => gameConfig.First ().GameConfigId)
				.Select (gameConfig => new GameConfig {
					Id = gameConfig.First ().GameConfigId,
						NumberOfPlayers = gameConfig.Max (pr => pr.PlayerId) - gameConfig.Min (pr => pr.PlayerId) + 1,
						NumberOfRounds = gameConfig.Max (pr => pr.RoundNum)
					//, lastRound = db.FindMaxRoundForGam
				}).ToList ();*/
		}

		public List<List<PlayerRound>> FindPlayerRoundsForGame (int gameConfigId) {
			var playerRounds = db.FindAll<PlayerRound> (pr => pr.GameConfigId == gameConfigId);
			return playerRounds.GroupBy (pr => pr.RoundNum)
				.OrderBy (round => round.First ().RoundNum)
				.Select (round => round.OrderBy (p => p.PlayerId).ToList ())
				.ToList ();
		}
	}
}