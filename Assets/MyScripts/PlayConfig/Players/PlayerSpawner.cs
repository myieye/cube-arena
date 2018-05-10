using System;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.GameObjects.Agents;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.PlayConfig.Players {

	public class PlayerSpawner : MonoBehaviourSingleton<PlayerSpawner> {

		[SerializeField]
		private GameObject cubeStartPointsPrefab;
		[SerializeField]
		private GameObject cubePrefab;
		[SerializeField]
		private GameObject playerPrefab;
		[SerializeField]
		private Vector3 spawnOffsetRatio;
		[SerializeField]
		public Material[] materials;
		private List<Transform> spawnPoints;
		private int nextSpawnPoint;

		public void SpawnPlayers (List<NetworkPlayer> players) {
			InitSpawnPoints (players);

			foreach (var netPlayer in players) {
				SpawnPlayerCubes (netPlayer);
			}
		}

		private void SpawnPlayerCubes (NetworkPlayer netPlayer) {
			if (!netPlayer.Spawned) {
				netPlayer.StartPosition = GetStartPosition ();
				netPlayer.PlayerGameObject = SpawnPlayerCursor (netPlayer);
				netPlayer.Spawned = true;
			}
			SpawnPlayerCubes (netPlayer.StartPosition, netPlayer);
		}

		private GameObject SpawnPlayerCursor (NetworkPlayer netPlayer) {
			var player = (GameObject) GameObject.Instantiate (playerPrefab);

			netPlayer.Color = PlayerSpawner.Instance.materials[netPlayer.PlayerNum - 1].color;
			var transparentColor = Highlight.ReduceTransparency (netPlayer.Color, Highlight.CursorTransparency);
			player.GetComponent<Colourer> ().color = transparentColor;

			player.GetComponent<PlayerId> ().Id = netPlayer.PlayerId;
			var device = netPlayer.DeviceConfig.Device;
			NetworkServer.AddPlayerForConnection (device.Connection, player, device.ControllerId);
			return player;
		}

		private void SpawnPlayerCubes (Transform startPos, NetworkPlayer netPlayer) {
			var cubeStartPoints = Instantiate (cubeStartPointsPrefab, startPos.position, startPos.rotation);

			var i = 1;
			foreach (Transform trans in cubeStartPoints.transform) {
				var cube = Instantiate (cubePrefab, trans.position, trans.rotation);
				cube.GetComponent<Rigidbody> ().maxAngularVelocity = Settings.Instance.MaxRotationVelocity;
				cube.GetComponent<Colourer> ().color = netPlayer.Color;
				cube.GetComponent<PlayerId> ().Id = netPlayer.PlayerNum;
				cube.name += i++;
				NetworkServer.SpawnWithClientAuthority (cube, netPlayer.PlayerGameObject);
			}
			Destroy (cubeStartPoints);
		}

		private void InitSpawnPoints (List<NetworkPlayer> players) {
			Assert.IsNotNull (players);
			Assert.IsTrue (players.Count > 0);
			Assert.IsTrue (players.Count (p => p == null) == 0);

			if (!players[0].Spawned) {
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
					Destroy (spawnPoint.gameObject);
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