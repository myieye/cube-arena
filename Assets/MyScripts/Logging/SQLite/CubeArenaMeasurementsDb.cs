using System;
using CubeArena.Assets.MyScripts.Constants;
using CubeArena.Assets.MyScripts.Logging.Models;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Logging.SQLite {
    public class CubeArenaMeasurementsDb {
        private SQLiteConnection conn;
        public CubeArenaMeasurementsDb () {
            conn = SQLiteDbService.CreateDb (Database.CubeArenaMeasurementsDatabase);
            TestDb ();
        }

        private void TestDb () {
            //*
            conn.DropTable<Move> ();
            conn.DropTable<Rotation> ();
            conn.DropTable<SelectionAction> ();
            conn.DropTable<Selection> ();
            conn.DropTable<Placement> ();
            conn.DropTable<Kill> ();
            conn.DropTable<Assist> ();
            //*/
            conn.CreateTable<Move> ();
            conn.CreateTable<Rotation> ();
            conn.CreateTable<SelectionAction> ();
            conn.CreateTable<Selection> ();
            conn.CreateTable<Placement> ();
            conn.CreateTable<Kill> ();
            conn.CreateTable<Assist> ();

            PrintTable<Move> ();
            PrintTable<Rotation> ();
            PrintTable<SelectionAction> ();
            PrintTable<Selection> ();
            PrintTable<Placement> ();
            PrintTable<Kill> ();
            PrintTable<Assist> ();
        }

        internal object InsertAssist (Assist assist) {
            return Insert(assist);
        }

        internal object InsertPlacement (Placement placement) {
            return Insert (placement);
        }

        internal object InsertSelection (Selection selection) {
            return Insert (selection);
        }

        internal object InsertSelectionAction (SelectionAction selectionAction) {
            return Insert (selectionAction);
        }

        internal object InsertKill (Kill kill) {
            return Insert (kill);
        }

        public Move InsertMove (Move move) {
            return Insert (move);
        }

        public Rotation InsertRotation (Rotation rotation) {
            return Insert (rotation);
        }

        private void PrintTable<T> () where T : new () {
            Debug.Log (typeof (T).Name + "s: ");
            foreach (var e in conn.Table<T> ()) {
                Debug.Log (e);
            }
        }

        private T Insert<T> (T entity) where T : Measurement {
            entity.Id = conn.Insert (entity);
            return entity;
        }
    }
}