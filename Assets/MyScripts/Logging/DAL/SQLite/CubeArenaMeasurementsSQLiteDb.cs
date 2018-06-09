using System;
using System.Linq;
using System.Linq.Expressions;
using CubeArena.Assets.MyScripts.Logging.DAL.Models;
using CubeArena.Assets.MyScripts.Logging.DAL.Models.Answers;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Logging.DAL.SQLite {
    public class CubeArenaMeasurementsSQLiteDb : CubeArenaMeasurementsDb {

        private SQLiteConnection conn;

        public CubeArenaMeasurementsSQLiteDb (DatabaseVersion dbVersion) {
            var dbName = string.Format ("{0}_{1}", Database.CubeArenaMeasurementsDatabase, dbVersion);
            conn = SQLiteUtil.CreateDb (dbName);
            TestDb ();
        }

        private void TestDb () {
            if (Settings.Instance.LogMeasurementsToConsole) {
                PrintTable<PlayerRound> ();
                PrintTable<Move> ();
                PrintTable<Rotation> ();
                PrintTable<SelectionAction> ();
                PrintTable<Selection> ();
                PrintTable<Placement> ();
                PrintTable<Kill> ();
                PrintTable<Assist> ();
                PrintTable<PlayerCounter> ();
                PrintTable<AreaInteraction> ();
                PrintTable<CloudMeasurement> ();
                PrintTable<Device> ();
                PrintTable<RatingAnswer> ();
                PrintTable<WeightAnswer> ();
            }

            if (Settings.Instance.ResetDbOnStart) {
                Debug.LogWarning ("Clearing database...");
                //*
                conn.DropTable<PlayerRound> ();
                conn.DropTable<Move> ();
                conn.DropTable<Rotation> ();
                conn.DropTable<SelectionAction> ();
                conn.DropTable<Selection> ();
                conn.DropTable<Placement> ();
                conn.DropTable<Kill> ();
                conn.DropTable<Assist> ();
                conn.DropTable<PlayerCounter> ();
                conn.DropTable<AreaInteraction> ();
                conn.DropTable<CloudMeasurement> ();
                conn.DropTable<Device> ();
                conn.DropTable<RatingAnswer> ();
                conn.DropTable<WeightAnswer> ();
                //*/
            }

            conn.CreateTable<PlayerRound> ();
            conn.CreateTable<Move> ();
            conn.CreateTable<Rotation> ();
            conn.CreateTable<SelectionAction> ();
            conn.CreateTable<Selection> ();
            conn.CreateTable<Placement> ();
            conn.CreateTable<Kill> ();
            conn.CreateTable<Assist> ();
            conn.CreateTable<PlayerCounter> ();
            conn.CreateTable<AreaInteraction> ();
            conn.CreateTable<CloudMeasurement> ();
            conn.CreateTable<Device> ();
            conn.CreateTable<RatingAnswer> ();
            conn.CreateTable<WeightAnswer> ();
        }

        public int GetNextPlayerId () {
            var maxList = conn.Query<PlayerCounter> ("select * from PlayerCounter");
            int newPlayerCount = 1;
            if (maxList.Count > 1) {
                throw new ApplicationException ("More than one player counter found!");
            } else if (maxList.Count == 1) {
                newPlayerCount = maxList[0].PlayerCount + 1;
                conn.Query<int> ("update PlayerCounter set PlayerCount = ?", newPlayerCount);
            } else {
                conn.Query<int> ("insert into PlayerCounter values (?)", newPlayerCount);
            }
            return newPlayerCount;
        }

        public T Find<T> (Expression<Func<T, bool>> condition) where T : BaseEntity, new () {
            return conn.Find<T> (condition);
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

        private void PrintTable<T> () where T : new () {
            Debug.Log (typeof (T).Name + "s: ");

            try {
                foreach (var e in conn.Table<T> ()) {
                    Debug.Log (e);
                }
            } catch {
                Debug.LogError ("Table does not exist: " + typeof (T).Name);
            }
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
    }
}