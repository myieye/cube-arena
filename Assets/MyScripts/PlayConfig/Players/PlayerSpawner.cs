using System;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.GameObjects.Agents;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Utils.Colors;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using CubeArena.Assets.MyScripts.Utils.Settings;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.PlayConfig.Players {

	public class PlayerSpawner : MonoBehaviourSingleton<PlayerSpawner> {

		[SerializeField]
		private GameObject cursorPrefab;
		[SerializeField]
		private GameObject cubePrefab;
		[SerializeField]
		private GameObject cubeStartPointsPrefab;
		[SerializeField]
		private Vector3 spawnOffsetRatio;
		[SerializeField]
		public PlayerColor[] colors;
		private List<Transform> spawnPoints;
		private int nextSpawnPoint;

		public void SpawnPlayers (List<NetworkPlayer> players) {
			InitSpawnPoints (players);

			foreach (var netPlayer in players) {
				SpawnPlayer (netPlayer);
			}
		}

		private void SpawnPlayer (NetworkPlayer netPlayer) {
			if (!netPlayer.AddedPlayer) {
				netPlayer.StartPosition = GetStartPosition ();
				netPlayer.Color = colors[netPlayer.PlayerIndex];
				netPlayer.AddedPlayer = true;
			}
			netPlayer.Cursor = SpawnPlayerCursor (netPlayer);
			netPlayer.Cubes = SpawnPlayerCubes (netPlayer.StartPosition, netPlayer);
		}

		private GameObject SpawnPlayerCursor (NetworkPlayer netPlayer) {
			var cursor = Instantiate (cursorPrefab);

			var transparentColor = Highlight.ReduceTransparency (netPlayer.Color.value, Highlight.CursorTransparency);
			cursor.GetComponent<Colourer> ().color = transparentColor;
			cursor.GetComponent<PlayerId> ().Id = netPlayer.PlayerId;
			cursor.name = Text.CursorName (netPlayer);
			NetworkServer.SpawnWithClientAuthority (cursor, netPlayer.DeviceConfig.Device.Connection);

			return cursor;
		}

		private List<GameObject> SpawnPlayerCubes (Transform startPos, NetworkPlayer netPlayer) {
			var cubeStartPoints = Instantiate (cubeStartPointsPrefab, startPos.position, startPos.rotation);

			var i = 1;
			var cubes = new List<GameObject> ();
			foreach (Transform trans in cubeStartPoints.transform) {
				var cube = Instantiate (cubePrefab, trans.position, trans.rotation);
				cube.GetComponent<Rigidbody> ().maxAngularVelocity = Settings.Instance.MaxRotationVelocity;
				cube.GetComponent<Colourer> ().color = netPlayer.Color.value;
				cube.GetComponent<PlayerId> ().Id = netPlayer.PlayerId;
				cube.name = Text.CubeName (netPlayer, i++);
				NetworkServer.SpawnWithClientAuthority (cube, netPlayer.DeviceConfig.Device.Connection);
				cubes.Add (cube);
			}
			Destroy (cubeStartPoints);

			return cubes;
		}

		private void InitSpawnPoints (List<NetworkPlayer> players) {
			Assert.IsNotNull (players);
			Assert.IsTrue (players.Count > 0);
			Assert.IsTrue (players.Count (p => p == null) == 0);

			if (!players[0].AddedPlayer) {
				nextSpawnPoint = 0;

				if (spawnPoints == null || spawnPoints.Count != players.Count) {
					DestroySpawnPoints ();
					GenerateSpawnPoints (players.Count);
				}
			}
		}

		private void DestroySpawnPoints () {
			if (spawnPoints != null) {
				foreach (var spawnPoint in spawnPoints) {
					if (spawnPoint) {
						Destroy (spawnPoint.gameObject);
					}
				}
			}
		}

		private void GenerateSpawnPoints (int count) {
			spawnPoints = new List<Transform> ();
			float spacing = 360f / count;
			var spawnOffset = spawnOffsetRatio * TransformUtil.LocalRadius;
			Vector3 heightenedCenter = new Vector3 (0, spawnOffset.y, 0);

			for (var i = 0; i < count; i++) {
				var spawnPoint = new GameObject ("SpawnPoint_" + (i + 1));
				spawnPoint.transform.Translate (spawnOffset);
				spawnPoint.transform.LookAt (heightenedCenter);
				spawnPoint.transform.RotateAround (Vector3.zero, Vector3.up, spacing * i);
				spawnPoints.Add (spawnPoint.transform);
			}
		}

		private Transform GetStartPosition () {
			var spawnPoint = spawnPoints[nextSpawnPoint].transform;
			nextSpawnPoint = (nextSpawnPoint + (int) Mathf.Floor (spawnPoints.Count / 2));
			if (nextSpawnPoint >= spawnPoints.Count &&
				spawnPoints.Count % (int) Mathf.Floor (spawnPoints.Count / 2) == 0) {
				nextSpawnPoint++;
			}
			nextSpawnPoint = nextSpawnPoint % spawnPoints.Count;
			return spawnPoint;
		}
	}
}