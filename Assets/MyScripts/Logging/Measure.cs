using System;
using System.Linq;
using CubeArena.Assets.MyPrefabs.Cursor;
using CubeArena.Assets.MyScripts.Agents;
using CubeArena.Assets.MyScripts.Constants;
using CubeArena.Assets.MyScripts.Data.Models;
using CubeArena.Assets.MyScripts.Logging.Models;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.Rounds;
using CubeArena.Assets.MyScripts.UI.Mode;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Logging {

    public class Measure : NetworkBehaviour {

        public ServerLogger logger;
        private static ServerLogger Logger { get; set; }
        private static GameObjectState movingObjStartState;
        private static GameObjectState movingObjCurrState;
        private static GameObjectState rotatingObjStartState;
        private static GameObjectState rotatingObjCurrState;
        private static float cumulativeMoveDistance;
        private static float cumulativeRotationAngle;
        private static DateTime selectionStart;
        private static int? interactionArea;
        private static DateTime areaInteractionStart;
        private static int? _playerRoundId;
        private static int PlayerRoundId {
            get {
                if (!_playerRoundId.HasValue) {
                    var playerId = FindObjectOfType<Network.PlayerId> ().Id;
                    _playerRoundId = PlayerManager.Instance.GetPlayerRoundId (playerId);
                }
                return _playerRoundId.Value;
            }
        }
        private static int? _playerId;
        private static int PlayerId {
            get {
                if (!_playerId.HasValue) {
                    _playerId = FindObjectOfType<Network.PlayerId> ().Id;
                }
                return _playerId.Value;
            }
        }
        private static UIModeManager _uiModeManager;
        private static int UIMode {
            get {
                if (!_uiModeManager) {
                    _uiModeManager = FindObjectOfType<UIModeManager> ();
                }
                return (int) _uiModeManager.CurrentUIMode;
            }
        }
        private static CursorController CursorCtrl {
            get {
                return Logger.GetComponent<CursorController> ();
            }
        }
        private static RoundManager RoundManager {
            get {
                return FindObjectOfType<RoundManager>();
            }
        }

        void Start () {
            Logger = logger;
            interactionArea = -1;
        }

        public static void StartMove (GameObject obj) {
            movingObjStartState = movingObjCurrState = new GameObjectState (obj);
            cumulativeMoveDistance = 0;
        }

        public static void UpdateMove (GameObject obj) {
            var newState = new GameObjectState (obj);
            cumulativeMoveDistance += Vector3.Distance (movingObjCurrState.Position, newState.Position);
            movingObjCurrState = newState;
        }

        public static void EndMove (GameObject obj) {
            UpdateMove (obj);
            Logger.CmdLogMove (Calc.CalcMove (cumulativeMoveDistance,
                movingObjStartState, movingObjCurrState, PlayerRoundId));

            var placedOn = CursorCtrl.GetAlignedWith ();
            var pointingUp = Calc.IsAlignedUp (CursorCtrl.gameObject);
            var placedOnCube = pointingUp && placedOn.CompareTag (Tags.Cube);
            int? placedOnPlayerId = null;
            if (placedOnCube) {
                placedOnPlayerId = placedOn.GetComponent<PlayerId> ().Id;
            }
            Logger.CmdLogPlacement (Calc.BuildPlacement (placedOnPlayerId, PlayerRoundId));
        }

        public static void StartRotation (GameObject obj) {
            rotatingObjStartState = rotatingObjCurrState = new GameObjectState (obj);
            cumulativeRotationAngle = 0;
        }

        public static void UpdateRotation (GameObject obj) {
            var newState = new GameObjectState (obj);
            cumulativeRotationAngle += Quaternion.Angle (rotatingObjCurrState.Rotation, newState.Rotation);
            rotatingObjCurrState = newState;
        }

        public static void EndRotation (GameObject obj) {
            UpdateRotation (obj);
            Logger.CmdLogRotation (Calc.CalcRotate (cumulativeRotationAngle,
                rotatingObjStartState, rotatingObjCurrState, PlayerRoundId));
        }

        public static void MadeKill (GameObject cube, Enemy enemy) {
            Logger.CmdLogKill (
                Calc.BuildKill (enemy, PlayerRoundId),
                Calc.CalcAssists (cube).ToArray ());
        }

        public static void MadeSelection (SelectionActionType type) {
            MeasureSelection (type);
            Logger.CmdLogSelectionAction (Calc.BuildSelectionAction (type, PlayerRoundId));
        }

        private static void MeasureSelection (SelectionActionType type) {
            if (SelectionActionType.Deselect.Equals (type)) {
                Logger.CmdLogSelection (Calc.BuildSelection (selectionStart, DateTime.Now, PlayerRoundId));
            } else if (SelectionActionType.Reselect.Equals (type)) {
                Logger.CmdLogSelection (Calc.BuildSelection (selectionStart, DateTime.Now, PlayerRoundId));
                selectionStart = DateTime.Now;
            } else if (SelectionActionType.Select.Equals (type)) {
                selectionStart = DateTime.Now;
            }
        }

        public static void UpdateInteractionArea (Vector3? interactionPoint) {
            //if (RoundManager.InPracticeMode) return;

            int area = Calc.CalcArea(interactionPoint,
                PlayerManager.Instance.GetPlayerStartPosition(PlayerId));
            if (interactionArea.HasValue && interactionArea.Value != area) {
                Debug.Log("Entered area: " + area);
                AreaInteraction areaInteraction = Calc.CalcAreaInteraction(
                    areaInteractionStart, DateTime.Now, interactionArea.Value, PlayerRoundId);
                Logger.CmdLogAreaInteraction(areaInteraction);
                areaInteractionStart = DateTime.Now;
            } else if (!interactionArea.HasValue) {
                areaInteractionStart = DateTime.Now;
            }
            interactionArea = area;
        }

        public static void FlushMeasurements () {
            rotatingObjStartState = null;
            movingObjStartState = null;
        }
    }
}