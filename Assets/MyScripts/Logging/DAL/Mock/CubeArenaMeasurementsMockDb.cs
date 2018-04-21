using System;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.Logging.DAL.Models;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Logging.DAL.Mock {
    public class CubeArenaMeasurementsMockDb : CubeArenaMeasurementsDb {

        private int nextPlayerId = 1;
        private Dictionary<Type, List<object>> entities = new Dictionary<Type, List<object>>();
        private Dictionary<Type, int> nextIds = new Dictionary<Type, int>();

        public int GetNextPlayerId () {
            return nextPlayerId++;
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

        public Device InsertDevice (Device device) {
            return Insert (device);
        }

        public Device GetDeviceByModel (string model) {
            var t = typeof(Device);
            CheckLists(t);
            return (Device) entities[t].Where(d => ((Device) d).Model.Equals(model)).FirstOrDefault();
        }

        private T Insert<T> (T entity) where T : BaseEntity {
            var t = typeof(T);
            CheckLists(t);
            entities[t].Add(entity);
            entity.Id = nextIds[t]++;
            return entity;
        }

        private void CheckLists(Type type) {
            if (!entities.ContainsKey(type)) {
                entities.Add(type, new List<object>());
            }
            if (!nextIds.ContainsKey(type)) {
                nextIds.Add(type, 1);
            }
        }
    }
}