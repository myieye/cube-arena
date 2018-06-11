using System;
using System.Collections.Generic;
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

    [RequireComponent (typeof (ServerLogger))]
    public class Measure : NetworkBehaviour {

        private ServerLogger logger;
        private GameObjectState movingObjStartState;
        private GameObjectState movingObjCurrState;
        private GameObjectState rotatingObjStartState;
        private GameObjectState rotatingObjCurrState;

        private float cumulativeMoveDistance;
        private float cumulativeRotationAngle;
        private DateTime selectionStart;
        private int? interactionArea;
        private DateTime areaInteractionStart;
        private List<Coroutine> tentativeSelectionCoroutines;
        public EnabledComponent<CursorController> Cursor { get; set; }
        public Vector3 StartPosition { get; set; }
        /*get {
                return GameObjectUtil.FindLocalAuthoritativeObject<StartPositionTracker> ().StartPosition;
            }
        }*/

        public static Measure LocalInstance { get; private set; }
        public static Measure ServerInstance { get; private set; }

        public override void OnStartAuthority () {
            LocalInstance = this;
            tentativeSelectionCoroutines = new List<Coroutine> ();

            if (GetComponent<NetworkIdentity> ().serverOnly) {
                ServerInstance = this;
            }
        }

        private void Start () {
            logger = GetComponent<ServerLogger> ();
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
            var move = Calc.CalcMove (cumulativeMoveDistance, movingObjStartState, movingObjCurrState);
            logger.CmdLogMove (move, move.Time);

            var cubeBelow = RayUtil.FindGameObjectBelow (obj.transform, Layers.CubesMask);
            // cursor.Get.GetAlignedWith ();
            var pointingUp = Calc.IsAlignedUp (Cursor.Get.gameObject);
            var placedOnCube = cubeBelow && (pointingUp || UIModeManager.InTouchMode);
            int placedOnPlayerId = -1;
            if (placedOnCube) {
                placedOnPlayerId = cubeBelow.GetComponent<PlayerId> ().Id;
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
            var rotation = Calc.CalcRotate (cumulativeRotationAngle,
                rotatingObjStartState, rotatingObjCurrState);
            logger.CmdLogRotation (rotation, rotation.Time);
        }

        public void MadeKill (GameObject killer, Enemy enemy) {
            if (Settings.Instance.ServerOnlyMeasurementLogging && !isServer) return;

            logger.CmdLogKill (
                Calc.BuildKill (enemy, killer),
                Calc.CalcAssists (killer).ToArray ());
        }

        public void MadeSelection (SelectionActionType type) {
            if (Settings.Instance.ServerOnlyMeasurementLogging && !isServer) return;

            MeasureSelection (type);
            logger.CmdLogSelectionAction (Calc.BuildSelectionAction (type));
        }

        public void MadeTentativeSelection (SelectionActionType type) {
            lock (this) {
                tentativeSelectionCoroutines.Add (
                    StartCoroutine (DelayUtil.Do (0.5f, () => {
                        lock (this) {
                            MadeSelection (type);
                            if (tentativeSelectionCoroutines.Any ()) {
                                tentativeSelectionCoroutines.RemoveAt (0);
                            }
                        }
                    })));
            }
        }

        public void CancelTentativeSelections () {
            lock (this) {
                foreach (var tentativeSelectionCoroutine in tentativeSelectionCoroutines) {
                    StopCoroutine (tentativeSelectionCoroutine);
                }
                tentativeSelectionCoroutines.Clear ();
            }
        }

        public void CloudDestroyed (float overlapTime, float multipleOverlapTime, int numOverlaps) {
            if (Settings.Instance.ServerOnlyMeasurementLogging && !isServer) return;

            logger.CmdLogCloudMeasurement (Calc.BuildCloudMeasurement (overlapTime, multipleOverlapTime, numOverlaps));
        }

        private void MeasureSelection (SelectionActionType type) {
            if (Settings.Instance.ServerOnlyMeasurementLogging && !isServer) return;

            if (SelectionActionType.Deselect.Equals (type)) {
                var selection = Calc.BuildSelection (selectionStart, DateTime.Now);
                logger.CmdLogSelection (selection, selection.Time);
            } else if (SelectionActionType.Reselect.Equals (type)) {
                var selection = Calc.BuildSelection (selectionStart, DateTime.Now);
                logger.CmdLogSelection (selection, selection.Time);
                selectionStart = DateTime.Now;
            } else if (SelectionActionType.Select.Equals (type)) {
                selectionStart = DateTime.Now;
            }
        }

        public void UpdateInteractionArea (Vector3? interactionPoint) {
            if (Settings.Instance.ServerOnlyMeasurementLogging && !isServer) return;

            int area = Calc.CalcArea (interactionPoint, StartPosition);
            if (interactionArea.HasValue && interactionArea.Value != area) {
                SaveCurrentAreaInteraction ();
            } else if (!interactionArea.HasValue) {
                areaInteractionStart = DateTime.Now;
            }
            interactionArea = area;
        }

        private void SaveCurrentAreaInteraction () {
            AreaInteraction areaInteraction = Calc.CalcAreaInteraction (
                areaInteractionStart, DateTime.Now, interactionArea.Value);
            logger.CmdLogAreaInteraction (areaInteraction, areaInteraction.Time);
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