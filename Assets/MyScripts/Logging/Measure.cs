using System;
using System.Linq;
using CubeArena.Assets.MyPrefabs.Cursor;
using CubeArena.Assets.MyScripts.Agents;
using CubeArena.Assets.MyScripts.Constants;
using CubeArena.Assets.MyScripts.Logging.Models;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.UI.Mode;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Logging {

    public class Measure : NetworkBehaviour {

        public LocalLogger logger;
        private static LocalLogger Logger { get; set; }
        private static GameObjectState movingObj;
        private static GameObjectState rotatingObj;
        private static DateTime selectionStart;
        private static int? _playerId;
        private static int PlayerId {
            get {
                if (!_playerId.HasValue) {
                    _playerId = FindObjectOfType<Network.PlayerId>().Id;
                }
                return _playerId.Value;
            }
        }
        private static UIModeManager _uiModeManager;
        private static int UIMode {
            get {
                if (!_uiModeManager) {
                    _uiModeManager = FindObjectOfType<UIModeManager>();
                }
                return (int) _uiModeManager.CurrentUIMode;
            }
        }
        private static CursorController CursorCtrl {
            get {
                return Logger.GetComponent<CursorController>();
            }
        }

        void Start () {
            Logger = logger;
        }

        public static void StartMove (GameObject obj) {
            movingObj = new GameObjectState (obj);
        }

        public static void EndMove (GameObject obj) {
            var newState = new GameObjectState (obj);
            Logger.CmdLogMove (Calc.CalcMove (movingObj, newState, PlayerId, UIMode));

            var placedOn = CursorCtrl.GetAlignedWith();
            var pointingUp = Calc.IsAlignedUp(CursorCtrl.gameObject);
            var placedOnCube = pointingUp && placedOn.CompareTag(Tags.Cube);
            int? placedOnPlayerId = null;
            if (placedOnCube) {
                placedOnPlayerId = placedOn.GetComponent<PlayerId>().Id;
            }
            Logger.CmdLogPlacement(Calc.BuildPlacement (placedOnPlayerId, PlayerId, UIMode));
        }

        public static void StartRotation (GameObject obj) {
            rotatingObj = new GameObjectState (obj);
        }

        public static void EndRotation (GameObject obj) {
            var newState = new GameObjectState (obj);
            Logger.CmdLogRotation (Calc.CalcRotate (rotatingObj, newState, PlayerId, UIMode));
        }

        public static void MadeKill(GameObject cube, Enemy enemy) {
            Logger.CmdLogKill(
                Calc.BuildKill(enemy, PlayerId),
                Calc.CalcAssists(cube).ToArray());
        }

        public static void MadeSelection(SelectionActionType type) {
            MeasureSelection(type);
            Logger.CmdLogSelectionAction(Calc.BuildSelectionAction(type, PlayerId, UIMode));
        }

        private static void MeasureSelection(SelectionActionType type) {
            if (SelectionActionType.Deselect.Equals(type)) {
                Logger.CmdLogSelection(Calc.BuildSelection(selectionStart, DateTime.Now, PlayerId, UIMode));
            } else if (SelectionActionType.Reselect.Equals(type)) {
                Logger.CmdLogSelection(Calc.BuildSelection(selectionStart, DateTime.Now, PlayerId, UIMode));
                selectionStart = DateTime.Now;
            } else if (SelectionActionType.Select.Equals(type)) {
                selectionStart = DateTime.Now;
            }
        }
    }
}