using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CubeArena.Assets.MyScripts.Logging.DAL.Models;
using CubeArena.Assets.MyScripts.Logging.DAL.Models.Answers;
using CubeArena.Assets.MyScripts.Logging.DAL.Models.Counters;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Logging.DAL.Mock {
    public class CubeArenaMeasurementsMockDb : CubeArenaMeasurementsDb {

        private Dictionary<Type, List<BaseEntity>> entities = new Dictionary<Type, List<BaseEntity>> ();
        private Dictionary<Type, int> nextIds = new Dictionary<Type, int> ();

        public int GetNextId<T> () where T : Counter, new () {
            var t = typeof (T);
            CheckLists (t);
            return nextIds[t]++;
        }

        public T Find<T> (Expression<Func<T, bool>> condition) where T : BaseEntity, new () {
            var t = typeof (T);
            CheckLists (t);
            return (T) entities[t].FirstOrDefault (e => condition.Compile () ((T) e));
        }

        public GameConfig InsertGameConfig (GameConfig gameConfig) {
            return Insert (gameConfig);
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

        public CloudMeasurement InsertCloudMeasurement (CloudMeasurement cloudMeasurement) {
            return Insert (cloudMeasurement);
        }

        public Device InsertDevice (Device device) {
            return Insert (device);
        }

        public RatingAnswer SaveRatingAnswer (RatingAnswer answer) {
            return UpdateOrInsert (answer);
        }

        public WeightAnswer SaveWeightAnswer (WeightAnswer answer) {
            return UpdateOrInsert (answer);
        }

        public Device GetDeviceByModel (string model) {
            var t = typeof (Device);
            CheckLists (t);
            return (Device) entities[t].Where (d => ((Device) d).Model.Equals (model)).FirstOrDefault ();
        }

        private T UpdateOrInsert<T> (T entity) where T : BaseEntity {
            if (entity.Id != default (int)) {
                var t = typeof (T);
                entities[t].RemoveAll (e => e.Id == entity.Id);
            }

            return Insert (entity);
        }

        private T Insert<T> (T entity) where T : BaseEntity {
            var t = typeof (T);
            CheckLists (t);
            entities[t].Add (entity);
            if (entity.Id == default (int)) {
                entity.Id = nextIds[t]++;
            }
            return entity;
        }

        private void CheckLists (Type type) {
            if (!entities.ContainsKey (type)) {
                entities.Add (type, new List<BaseEntity> ());
            }
            if (!nextIds.ContainsKey (type)) {
                nextIds.Add (type, 1);
            }
        }

        public void OnDestroy () { }

        public List<T> FindAll<T> (Expression<Func<T, bool>> condition = null) where T : new () {
            return new List<T> ();
        }
    }
}