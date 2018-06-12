using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CubeArena.Assets.MyScripts.Logging.DAL.Models;
using CubeArena.Assets.MyScripts.Logging.DAL.Models.Answers;
using CubeArena.Assets.MyScripts.Logging.DAL.Models.Counters;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Logging.DAL.SQLite {
    public class CubeArenaMeasurementsSQLiteDb : CubeArenaMeasurementsDb {

        private SQLiteConnection conn;

        public CubeArenaMeasurementsSQLiteDb (string dbName, bool resetIfExists, bool dumpTablesToConsole) {

            conn = SQLiteUtil.CreateDb (dbName);

            CreateTablesIfNotExist ();

            PrintTables (dumpTablesToConsole);

            if (resetIfExists) {
                DropTables ();
                CreateTablesIfNotExist ();
            }
        }

        private void PrintTables (bool printContents) {
            PrintTable<PlayerRound> (printContents);
            PrintTable<Move> (printContents);
            PrintTable<Rotation> (printContents);
            PrintTable<SelectionAction> (printContents);
            PrintTable<Selection> (printContents);
            PrintTable<Placement> (printContents);
            PrintTable<Kill> (printContents);
            PrintTable<Assist> (printContents);
            PrintTable<PlayerCounter> (printContents);
            PrintTable<GameConfig> (printContents);
            PrintTable<AreaInteraction> (printContents);
            PrintTable<CloudMeasurement> (printContents);
            PrintTable<Device> (printContents);
            PrintTable<RatingAnswer> (printContents);
            PrintTable<WeightAnswer> (printContents);
        }

        private void DropTables () {
            Debug.LogWarning ("Clearing database...");
            conn.DropTable<PlayerRound> ();
            conn.DropTable<Move> ();
            conn.DropTable<Rotation> ();
            conn.DropTable<SelectionAction> ();
            conn.DropTable<Selection> ();
            conn.DropTable<Placement> ();
            conn.DropTable<Kill> ();
            conn.DropTable<Assist> ();
            conn.DropTable<PlayerCounter> ();
            conn.DropTable<GameConfig> ();
            conn.DropTable<AreaInteraction> ();
            conn.DropTable<CloudMeasurement> ();
            conn.DropTable<Device> ();
            conn.DropTable<RatingAnswer> ();
            conn.DropTable<WeightAnswer> ();
        }

        private void CreateTablesIfNotExist () {
            conn.CreateTable<PlayerRound> ();
            conn.CreateTable<Move> ();
            conn.CreateTable<Rotation> ();
            conn.CreateTable<SelectionAction> ();
            conn.CreateTable<Selection> ();
            conn.CreateTable<Placement> ();
            conn.CreateTable<Kill> ();
            conn.CreateTable<Assist> ();
            conn.CreateTable<PlayerCounter> ();
            conn.CreateTable<GameConfig> ();
            conn.CreateTable<AreaInteraction> ();
            conn.CreateTable<CloudMeasurement> ();
            conn.CreateTable<Device> ();
            conn.CreateTable<RatingAnswer> ();
            conn.CreateTable<WeightAnswer> ();
        }

        public int GetNextId<T> () where T : Counter, new () {
            var name = typeof (T).Name;
            var maxList = conn.Query<T> (string.Format ("select * from {0}", name));
            int newPlayerCount = 1;
            if (maxList.Count > 1) {
                throw new ApplicationException (string.Format ("More than one {0}-counter found!", name));
            } else if (maxList.Count == 1) {
                newPlayerCount = maxList[0].Count + 1;
                conn.Query<int> (string.Format ("update {0} set Count = ?", name), newPlayerCount);
            } else {
                conn.Query<int> (string.Format ("insert into {0} values (?)", name), newPlayerCount);
            }
            return newPlayerCount;
        }

        public T Find<T> (Expression<Func<T, bool>> condition) where T : BaseEntity, new () {
            return conn.Find<T> (condition);
        }

        public List<T> FindAll<T> (Expression<Func<T, bool>> condition = null) where T : new () {
            if (condition != null) {
                return conn.Table<T> ().Where (condition).ToList ();
            } else {
                return conn.Table<T> ().ToList ();
            }
        }

        public GameConfig InsertGameConfig (GameConfig gameConfig) {
            return InsertOrUpdate (gameConfig);
        }

        public Assist InsertAssist (Assist assist) {
            return InsertOrUpdate (assist);
        }

        public PlayerRound InsertPlayerRound (PlayerRound playerRound) {
            return InsertOrUpdate (playerRound);
        }

        public Placement InsertPlacement (Placement placement) {
            return InsertOrUpdate (placement);
        }

        public Selection InsertSelection (Selection selection) {
            return InsertOrUpdate (selection);
        }

        public SelectionAction InsertSelectionAction (SelectionAction selectionAction) {
            return InsertOrUpdate (selectionAction);
        }

        public Kill InsertKill (Kill kill) {
            return InsertOrUpdate (kill);
        }

        public Move InsertMove (Move move) {
            return InsertOrUpdate (move);
        }

        public Rotation InsertRotation (Rotation rotation) {
            return InsertOrUpdate (rotation);
        }

        public AreaInteraction InsertAreaInteraction (AreaInteraction areaInteraction) {
            return InsertOrUpdate (areaInteraction);
        }

        public CloudMeasurement InsertCloudMeasurement (CloudMeasurement cloudMeasurement) {
            return InsertOrUpdate (cloudMeasurement);
        }

        public Device GetDeviceByModel (string model) {
            return conn.Table<Device> ().Where (d => d.Model.Equals (model)).FirstOrDefault ();
        }

        public Device InsertDevice (Device device) {
            return InsertOrUpdate (device, typeof (Device));
        }

        public RatingAnswer SaveRatingAnswer (RatingAnswer answer) {
            return InsertOrUpdate (answer);
        }

        public WeightAnswer SaveWeightAnswer (WeightAnswer answer) {
            return InsertOrUpdate (answer);
        }

        private T InsertOrUpdate<T> (T entity, Type type = null) where T : BaseEntity {
            if (entity.Id == default (int)) {
                if (type == null) {
                    conn.Insert (entity);
                } else {
                    conn.Insert (entity, type);
                }
            } else {
                if (type == null) {
                    var prev = entity.Id;
                    conn.Update (entity);
                } else {
                    conn.Update (entity, type);
                }
            }
            return entity;
        }

        private void PrintTable<T> (bool printContents) where T : new () {
            Debug.LogFormat ("{0}s: [{1}]", typeof (T).Name, conn.Table<T> ().Count ());

            if (printContents) {
                try {
                    foreach (var e in conn.Table<T> ()) {
                        Debug.Log (e);
                    }
                } catch {
                    Debug.LogError ("Table does not exist: " + typeof (T).Name);
                }
            }
        }

        public void OnDestroy () {
            if (conn != null) {
                conn.Close ();
                conn.Dispose ();
            }
        }
    }
}