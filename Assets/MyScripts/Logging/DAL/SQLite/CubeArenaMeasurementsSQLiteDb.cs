using System;
using System.Linq;
using CubeArena.Assets.MyScripts.Logging.DAL.Models;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Logging.DAL.SQLite {
    public class CubeArenaMeasurementsSQLiteDb : CubeArenaMeasurementsDb {

        private SQLiteConnection conn;

        public CubeArenaMeasurementsSQLiteDb () {
            conn = SQLiteUtil.CreateDb (Database.CubeArenaMeasurementsDatabase);
            TestDb ();
        }

        private void TestDb () {
            if (Settings.Instance.ResetDbOnStart) {
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
                conn.DropTable<Device> ();

                //*/
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
                conn.CreateTable<Device> ();
            }

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
                PrintTable<Device> ();
            }
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

        public Assist InsertAssist (Assist assist) {
            return Insert (assist);
        }

        public PlayerRound InsertPlayerRound (PlayerRound playerRound) {
            return Insert (playerRound);
        }

        public Placement InsertPlacement (Placement placement) {
            return Insert (placement);
        }

        public Selection InsertSelection (Selection selection) {
            return Insert (selection);
        }

        public SelectionAction InsertSelectionAction (SelectionAction selectionAction) {
            return Insert (selectionAction);
        }

        public Kill InsertKill (Kill kill) {
            return Insert (kill);
        }

        public Move InsertMove (Move move) {
            return Insert (move);
        }

        public Rotation InsertRotation (Rotation rotation) {
            return Insert (rotation);
        }

        public AreaInteraction InsertAreaInteraction (AreaInteraction areaInteraction) {
            return Insert (areaInteraction);
        }

        public Device GetDeviceByModel (string model) {
            return conn.Table<Device> ().Where (d => d.Model.Equals (model)).FirstOrDefault ();
        }

        public Device InsertDevice (Device device) {
            return Insert (device, typeof(Device));
        }

        private void PrintTable<T> () where T : new () {
            Debug.Log (typeof (T).Name + "s: ");
            foreach (var e in conn.Table<T> ()) {
                Debug.Log (e);
            }
        }

        private T Insert<T> (T entity, Type type = null) where T : BaseEntity {
            if (type == null) {
                entity.Id = conn.Insert (entity);
            } else {
                entity.Id = conn.Insert (entity, type);
            }
            return entity;
        }
    }
}