using System;
using System.Linq;
using CubeArena.Assets.MyPrefabs.Cursor;
using CubeArena.Assets.MyScripts.GameObjects.Agents;
using CubeArena.Assets.MyScripts.Logging.DAL.Models;
using CubeArena.Assets.MyScripts.Logging.Models;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using CubeArena.Assets.MyScripts.PlayConfig.Rounds;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Logging {

    public class Measure : NetworkBehaviour {

        public ServerLogger logger;
        private GameObjectState movingObjStartState;
        private GameObjectState movingObjCurrState;
        private GameObjectState rotatingObjStartState;
        private GameObjectState rotatingObjCurrState;

        private float cumulativeMoveDistance;
        private float cumulativeRotationAngle;
        private DateTime selectionStart;
        private int? interactionArea;
        private DateTime areaInteractionStart;
        private Coroutine tentativeSelectionCoroutine;
        /*private static UIModeManager _uiModeManager;
        private static int UIMode {
            get {
                if (!_uiModeManager) {
                    _uiModeManager = FindObjectOfType<UIModeManager> ();
                }
                return (int) _uiModeManager.CurrentUIMode;
            }
        }*/
        private CursorController CursorCtrl {
            get {
                return logger.GetComponent<CursorController> ();
            }
        }
        private RoundManager RoundManager {
            get {
                return FindObjectOfType<RoundManager> ();
            }
        }
        private Vector3 StartPosition {
            get {
                return logger.GetComponent<StartPositionTracker> ().StartPosition;
            }
        }

        public static Measure LocalInstance { get; private set; }

        public override void OnStartAuthority () {
            LocalInstance = this;
        }

        public void StartMove (GameObject obj) {
            if (Settings.Instance.ServerOnlyMeasurementLogging && !isServer) return;

            movingObjStartState = movingObjCurrState = new GameObjectState (obj);
            cumulativeMoveDistance = 0;
        }

        public void UpdateMove (GameObject obj) {
            if (Settings.Instance.ServerOnlyMeasurementLogging && !isServer) return;

            var newState = new GameObjectState (obj);
            cumulativeMoveDistance += Vector3.Distance (movingObjCurrState.Position, newState.Position);
            movingObjCurrState = newState;
        }

        public void EndMove (GameObject obj) {
            if (Settings.Instance.ServerOnlyMeasurementLogging && !isServer) return;

            UpdateMove (obj);
            logger.CmdLogMove (Calc.CalcMove (cumulativeMoveDistance, movingObjStartState, movingObjCurrState));

            var placedOn = CursorCtrl.GetAlignedWith ();
            var pointingUp = Calc.IsAlignedUp (CursorCtrl.gameObject);
            var placedOnCube = pointingUp && placedOn && placedOn.CompareTag (Tags.Cube);
            int? placedOnPlayerId = null;
            if (placedOnCube) {
                placedOnPlayerId = placedOn.GetComponent<PlayerId> ().Id;
            }
            logger.CmdLogPlacement (Calc.BuildPlacement (placedOnPlayerId));
        }

        public void StartRotation (GameObject obj) {
            if (Settings.Instance.ServerOnlyMeasurementLogging && !isServer) return;

            rotatingObjStartState = rotatingObjCurrState = new GameObjectState (obj);
            cumulativeRotationAngle = 0;
        }

        public void UpdateRotation (GameObject obj) {
            if (Settings.Instance.ServerOnlyMeasurementLogging && !isServer) return;

            var newState = new GameObjectState (obj);
            cumulativeRotationAngle += Quaternion.Angle (rotatingObjCurrState.Rotation, newState.Rotation);
            rotatingObjCurrState = newState;
        }

        public void EndRotation (GameObject obj) {
            if (Settings.Instance.ServerOnlyMeasurementLogging && !isServer) return;

            UpdateRotation (obj);
            logger.CmdLogRotation (Calc.CalcRotate (cumulativeRotationAngle,
                rotatingObjStartState, rotatingObjCurrState));
        }

        public void MadeKill (GameObject cube, Enemy enemy) {
            if (Settings.Instance.ServerOnlyMeasurementLogging && !isServer) return;

            logger.CmdLogKill (
                Calc.BuildKill (enemy),
                Calc.CalcAssists (cube).ToArray ());
        }

        public void MadeSelection (SelectionActionType type) {
            if (Settings.Instance.ServerOnlyMeasurementLogging && !isServer) return;

            MeasureSelection (type);
            logger.CmdLogSelectionAction (Calc.BuildSelectionAction (type));
        }

        public void MadeTentativeSelection (SelectionActionType type) {
            tentativeSelectionCoroutine =
                StartCoroutine (DelayUtil.Do (0.5f, () => {
                    MadeSelection (type);
                    tentativeSelectionCoroutine = null;
                }));
        }
        
        public void CancelTentativeSelection () {
            if (tentativeSelectionCoroutine != null) {
                StopCoroutine (tentativeSelectionCoroutine);
            }
        }

        public void CloudDestroyed (float overlapTime, float multipleOverlapTime, int numOverlaps) {
            if (Settings.Instance.ServerOnlyMeasurementLogging && !isServer) return;

            logger.CmdLogCloudMeasurement (Calc.BuildCloudMeasurement (overlapTime, multipleOverlapTime, numOverlaps));
        }

        private void MeasureSelection (SelectionActionType type) {
            if (Settings.Instance.ServerOnlyMeasurementLogging && !isServer) return;

            if (SelectionActionType.Deselect.Equals (type)) {
                logger.CmdLogSelection (Calc.BuildSelection (selectionStart, DateTime.Now));
            } else if (SelectionActionType.Reselect.Equals (type)) {
                logger.CmdLogSelection (Calc.BuildSelection (selectionStart, DateTime.Now));
                selectionStart = DateTime.Now;
            } else if (SelectionActionType.Select.Equals (type)) {
                selectionStart = DateTime.Now;
            }
        }

        public void UpdateInteractionArea (Vector3? interactionPoint) {
            if (Settings.Instance.ServerOnlyMeasurementLogging && !isServer) return;

            int area = Calc.CalcArea (interactionPoint, StartPosition);
            if (interactionArea.HasValue && interactionArea.Value != area) {
                //Debug.Log("Entered area: " + area);
                SaveCurrentAreaInteraction ();
            } else if (!interactionArea.HasValue) {
                areaInteractionStart = DateTime.Now;
            }
            interactionArea = area;
        }

        private void SaveCurrentAreaInteraction () {
            AreaInteraction areaInteraction = Calc.CalcAreaInteraction (
                areaInteractionStart, DateTime.Now, interactionArea.Value);
            logger.CmdLogAreaInteraction (areaInteraction);
            areaInteractionStart = DateTime.Now;
        }

        [ClientRpc]
        public void RpcFlushMeasurements () {
            if (hasAuthority && logger) {
                FlushMeasurements ();
            }
        }

        public void FlushMeasurements () {
            if (Settings.Instance.ServerOnlyMeasurementLogging && !isServer) return;

            SaveCurrentAreaInteraction ();
        }
    }
}