using System;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyPrefabs.Cubes;
using CubeArena.Assets.MyScripts.Agents;
using CubeArena.Assets.MyScripts.Constants;
using CubeArena.Assets.MyScripts.Data.Models;
using CubeArena.Assets.MyScripts.Fire;
using CubeArena.Assets.MyScripts.Logging.Models;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.Rounds;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Logging {
    public static class Calc {
        public static Move CalcMove (float cumulativeDistance, GameObjectState from, GameObjectState to) {
            return new Move {
                Distance = Vector3.Distance (from.Position, to.Position),
                    CumulativeDistance = cumulativeDistance,
                    Time = (to.Time - from.Time).TotalMilliseconds
            };
        }

        public static Rotation CalcRotate (float cumulativeRotation, GameObjectState from, GameObjectState to) {
            return new Rotation {
                Angle = Quaternion.Angle (from.Rotation, to.Rotation),
                    CumulativeAngle = cumulativeRotation,
                    Time = (to.Time - from.Time).TotalMilliseconds
            };
        }

        public static SelectionAction BuildSelectionAction (SelectionActionType type) {
            return new SelectionAction {
                Type = type
            };
        }

        public static Selection BuildSelection (DateTime from, DateTime to) {
            return new Selection {
                Time = (to - from).TotalMilliseconds
            };
        }

        public static Placement BuildPlacement (int? placedOnPlayerId) {
            return new Placement {
                PlacedOnPlayerId = placedOnPlayerId.HasValue ? placedOnPlayerId.Value : -1
            };
        }

        public static bool IsAlignedUp (GameObject gameObject) {
            return Quaternion.Angle (gameObject.transform.rotation, Quaternion.identity) < 45;
        }

        public static Kill BuildKill (Enemy enemy) {
            return new Kill {
                Level = enemy.level
            };
        }

        public static List<Assist> CalcAssists (GameObject hitter) {
            var hitFireCube = hitter.GetComponentInChildren<FireCube> ();
            var fireCubes = new List<FireCube> { hitFireCube };

            FindAttachedFireCubesRec (hitFireCube, fireCubes);
            var assistCubes = FindAssists (fireCubes);

            assistCubes.Remove (hitFireCube);
            var tippers = GetTippers (hitter, assistCubes);

            return BuildAssists (assistCubes.Except (tippers), tippers);
        }

        private static List<Assist> BuildAssists (IEnumerable<FireCube> stackerCubes, List<FireCube> tipperCubes) {
            var assists = new List<Assist> ();
            foreach (var assist in stackerCubes) {
                assists.Add (new Assist {
                    Type = AssistType.Stacker,
                        PlayerRoundId = GetPlayerRoundId (assist.Cube)
                });
            }
            foreach (var tipper in tipperCubes) {
                assists.Add (new Assist {
                    Type = AssistType.Tipper,
                        PlayerRoundId = GetPlayerRoundId (tipper.Cube)
                });
            }
            return assists;
        }

        private static List<FireCube> GetTippers (GameObject hitter, List<FireCube> assists) {
            return (from assist in assists where IsBelow (assist.gameObject, hitter) && IsRotating (assist) select assist).ToList ();
        }

        private static bool IsBelow (GameObject assist, GameObject hitter) {
            return assist.transform.position.y < hitter.transform.position.y;
        }

        private static bool IsRotating (FireCube assist) {
            return assist.Cube.GetComponent<CubeStateManager> ().IsRotating;
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

        private static List<FireCube> FindAssists (List<FireCube> cubes) {
            // Cubes that count as assists are:
            // (1) All attached cubes that are off the ground
            var offGroundAssists = new List<FireCube> ();
            foreach (var fireCube in cubes) {
                if (!FireCubeIsOnGround (fireCube)) {
                    offGroundAssists.Add (fireCube);
                }
            }

            // (2) All cubes on the ground attached to a cube off the grounf
            var allAssists = new List<FireCube> (offGroundAssists);
            foreach (var fireCube in cubes.Except (offGroundAssists)) {
                if (FireCubeIsAttachedToAny (fireCube, offGroundAssists)) {
                    allAssists.Add (fireCube);
                }
            }

            return allAssists;
        }

        public static int CalcArea (Vector3? interactionPoint, Vector3 playerStartPos) {
            if (interactionPoint.HasValue) {
                var intPoint = interactionPoint.Value;
                var maxMagnitude = playerStartPos.magnitude + Settings.Instance.AreaCenterPlayerStartPointOffset;
                intPoint.y = playerStartPos.y = 0;
                var areaCenter = Vector3.ClampMagnitude (playerStartPos * 1000, maxMagnitude);
                var distance = Vector3.Distance (areaCenter, intPoint);
                var areaIndex = Array.FindIndex (Settings.Instance.AreaRadiuses, r => r > distance);
                if (areaIndex >= 0) {
                    return areaIndex + 1;
                } else {
                    return Settings.Instance.AreaRadiuses.Length + 1;
                }
            } else {
                return 0;
            }
        }

        public static AreaInteraction CalcAreaInteraction (DateTime from,
            DateTime to, int area) {
            return new AreaInteraction {
                Area = area,
                    Time = (to - from).TotalMilliseconds
            };
        }

        private static bool FireCubeIsAttachedToAny (FireCube fireCube, List<FireCube> fireCubes) {
            var fires = from fc in fireCubes select fc.Cube;
            return fireCube.FireSources
                .Exists (fireSrc => fires.Contains (fireSrc.gameObject));
        }

        private static bool FireCubeIsOnGround (FireCube fireCube) {
            return fireCube.FireSources
                .Exists (fireSrc => fireSrc.gameObject.CompareTag (Tags.Ground));
        }
        
        private static int GetPlayerRoundId (GameObject playerObj) {
            var playerId = playerObj.GetComponent<PlayerId> ().Id;
            return PlayerManager.Instance.GetPlayerRoundId (playerId);
        }
    }
}