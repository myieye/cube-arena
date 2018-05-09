using System;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.GameObjects.Agents;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.PlayConfig.Players {

	public class PlayerSpawner : MonoBehaviourSingleton<PlayerSpawner> {

		public GameObject cubeStartPointsPrefab;
		public GameObject cubePrefab;
		public GameObject playerPrefab;
		public Material[] materials;
		private NetworkStartPosition[] spawnPoints;
		private int nextSpawnPoint;

		void Start () {
			spawnPoints = FindObjectsOfType<NetworkStartPosition> ();
			nextSpawnPoint = 0;
		}

		public void SpawnPlayers (List<NetworkPlayer> players) {
			foreach (var netPlayer in players) {
				SpawnPlayer (netPlayer);
			}
		}

		private void SpawnPlayer (NetworkPlayer netPlayer) {
			var startPos = GetStartPosition ();
			if (netPlayer.Spawned) {
				ConfigurePlayerCursor (netPlayer.PlayerGameObject, netPlayer);
			} else {
				netPlayer.PlayerGameObject = SpawnPlayerCursor (startPos, netPlayer);
			}
			netPlayer.StartPosition = startPos;
			SpawnCubesForPlayer (startPos, netPlayer);
		}

		private GameObject SpawnPlayerCursor (Transform startPos, NetworkPlayer netPlayer) {
			var player = (GameObject) GameObject.Instantiate (playerPrefab);
			ConfigurePlayerCursor (player, netPlayer);
			var device = netPlayer.DeviceConfig.Device;
			NetworkServer.AddPlayerForConnection (device.Connection, player, device.ControllerId);
			netPlayer.Spawned = true;
			return player;
		}

		private void ConfigurePlayerCursor (GameObject cursor, NetworkPlayer netPlayer) {
			var color = Highlight.ReduceTransparency (netPlayer.Color, Highlight.CursorTransparency);
			cursor.GetComponent<Colourer> ().color = color;
			cursor.GetComponent<PlayerId> ().Id = netPlayer.PlayerId;
		}

		private void SpawnCubesForPlayer (Transform startPos, NetworkPlayer netPlayer) {
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

		private Transform GetStartPosition () {
			return spawnPoints[nextSpawnPoint++ % spawnPoints.Length].transform;
		}
	}
}