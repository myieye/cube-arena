using System;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyPrefabs.Cubes;
using CubeArena.Assets.MyScripts.Agents;
using CubeArena.Assets.MyScripts.Constants;
using CubeArena.Assets.MyScripts.Fire;
using CubeArena.Assets.MyScripts.Logging.Models;
using CubeArena.Assets.MyScripts.Network;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Logging {
    public static class Calc {
        public static Move CalcMove (GameObjectState from, GameObjectState to, int playerId, int ui) {
            return new Move {
                Distance = Vector3.Distance (from.Position, to.Position),
                    Time = (to.Time - from.Time).TotalMilliseconds,
                    PlayerId = playerId,
                    UI = ui
            };
        }

        public static Rotation CalcRotate (GameObjectState from, GameObjectState to, int playerId, int ui) {
            return new Rotation {
                Angle = Quaternion.Angle (from.Rotation, to.Rotation),
                    Time = (to.Time - from.Time).TotalMilliseconds,
                    PlayerId = playerId,
                    UI = ui
            };
        }

        public static SelectionAction BuildSelectionAction (SelectionActionType type, int playerId, int ui) {
            return new SelectionAction {
                Type = type,
                    PlayerId = playerId,
                    UI = ui
            };
        }

        public static Selection BuildSelection (DateTime from, DateTime to, int playerId, int ui) {
            return new Selection {
                Time = (to - from).TotalMilliseconds,
                    PlayerId = playerId,
                    UI = ui
            };
        }

        public static Placement BuildPlacement (int? placedOnPlayerId, int playerId, int uIMode) {
            return new Placement {
                PlacedOnPlayerId = placedOnPlayerId.HasValue ? placedOnPlayerId.Value : -1,
                PlayerId = playerId,
                UI = uIMode
            };
        }

        public static bool IsAlignedUp (GameObject gameObject) {
            return Quaternion.Angle (gameObject.transform.rotation, Quaternion.identity) < 45;
        }

        public static Kill BuildKill(Enemy enemy, int playerId) {
            return new Kill {
                Level = enemy.level,
                PlayerId = playerId
            };
        }

        public static List<Assist> CalcAssists (GameObject hitter) {
            var hitFireCube = hitter.GetComponentInChildren<FireCube> ();
            var cubes = new List<FireCube> { hitFireCube };
            FindAttachedFireCubesRec (hitFireCube, cubes);

            Debug.Log("Cubes used: " + cubes.Count);

            var assists = GetAssists (cubes);
            assists.Remove (hitFireCube);
            var tippers = GetTippers (hitter, assists);
            
            return (from assist in assists
                select  BuildAssist(assist.gameObject.transform.parent.gameObject,
                tippers.Contains(assist) ? AssistType.Tipper : AssistType.Stacker)).ToList();
        }

        private static Assist BuildAssist(GameObject assist, AssistType assistType) {
            return new Assist {
                Type = assistType,
                PlayerId = assist.GetComponent<PlayerId>().Id
            };
        }

        private static List<FireCube> GetTippers (GameObject hitter, List<FireCube> assists) {
            return
            (from assist in assists
            where IsBelow (assist.gameObject, hitter) && IsRotating (assist)
            select assist).ToList();
        }

        private static bool IsBelow (GameObject assist, GameObject hitter) {
            return assist.transform.position.y < hitter.transform.position.y;
        }

        private static bool IsRotating (FireCube assist) {
            return assist.GetComponentInParent<CubeStateManager>().IsRotating;
        }

        private static void FindAttachedFireCubesRec (FireCube source, List<FireCube> cubes) {
            foreach (var fireSrc in source.FireSources) {
                if (fireSrc.gameObject.CompareTag (Tags.Cube)) {
                    var fireCube = fireSrc.gameObject.GetComponentInChildren<FireCube> ();
                    if (!cubes.Contains (fireCube)) {
                        cubes.Add (fireCube);
                        FindAttachedFireCubesRec (source, cubes);
                    }
                }
            }
        }

        private static List<FireCube> GetAssists (List<FireCube> cubes) {
            var offGroundAssists = new List<FireCube> ();
            foreach (var fireCube in cubes) {
                if (!FireCubeIsOnGround (fireCube)) {
                    offGroundAssists.Add (fireCube);
                }
            }
            var allAssists = new List<FireCube> (offGroundAssists);
            foreach (var fireCube in cubes.Except (offGroundAssists)) {
                Debug.Log("Checking for on ground: ");
                if (FireCubeAttachedToAny (fireCube, offGroundAssists)) {
                    Debug.Log("FireCubeAttachedToAny");
                    allAssists.Add (fireCube);
                }
            }
            return allAssists;
        }

        private static bool FireCubeAttachedToAny (FireCube fireCube, List<FireCube> fireCubes) {
            var fires = from fc in fireCubes select fireCube.gameObject.transform.parent.gameObject;
            Debug.Log("Equals: " + fireCubes.Contains(fireCube));
            Debug.Log("Fires");
            foreach (var f in fires) {
                Debug.Log(f);
                Debug.Log(f.GetInstanceID());
            }
            Debug.Log("Fource Sources");
            return fireCube.FireSources
                .Exists (fs => {
                    Debug.Log(fs.gameObject);
                    Debug.Log(fs.gameObject.GetInstanceID());
                    return fires.Contains (fs.gameObject);
                });
        }

        private static bool FireCubeIsOnGround (FireCube fireCube) {
            return fireCube.FireSources
                .Exists (fs => fs.gameObject.CompareTag (Tags.Ground));
        }
    }
}