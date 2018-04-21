using System;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.GameObjects.Agents;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils;
using UnityEngine;
using UnityEngine.Networking;
using CubeArena.Assets.MyScripts.Utils.Helpers;

namespace CubeArena.Assets.MyScripts.PlayConfig.Players {

	public class PlayerSpawner : MonoBehaviourSingleton<PlayerSpawner> {

		public GameObject cubeStartPoints;
		public GameObject cubePrefab;
		public GameObject playerPrefab;
		public Material[] materials;

		private ARManager arManager;
		private NetworkStartPosition[] spawnPoints;
		private int nextSpawnPoint;

		void Start () {
			if (Settings.Instance.AREnabled) {
				arManager = FindObjectOfType<ARManager> ();
			}
			spawnPoints = FindObjectsOfType<NetworkStartPosition>();
			nextSpawnPoint = 0;
		}

		public void SpawnPlayers (List<NetworkPlayer> players) {
			foreach (var netPlayer in players) {
				SpawnPlayer (netPlayer);
			}
		}

		private void SpawnPlayer (NetworkPlayer netPlayer) {
			var startPos = GetStartPosition ();
			netPlayer.PlayerGameObject = SpawnPlayerCursor (startPos, netPlayer, netPlayer.Color);
			netPlayer.StartPosition = startPos;
			SpawnCubesForPlayer (startPos, netPlayer, netPlayer.Color);
		}

		private GameObject SpawnPlayerCursor (Transform startPos, NetworkPlayer netPlayer, Color color) {
			var player = (GameObject) GameObject.Instantiate (playerPrefab);
			if (Settings.Instance.AREnabled) {
				arManager.AddARObjectToWorld (player.GetComponent<ARObject> ());
			}
			color = Highlight.ReduceTransparency (color, Highlight.CursorTransparency);
			player.GetComponent<Colourer> ().color = color;
			player.GetComponent<PlayerId> ().Id = netPlayer.PlayerNum;
			var device = netPlayer.DeviceConfig.Device;
			NetworkServer.AddPlayerForConnection (device.Connection, player, device.ControllerId);
			return player;
		}

		private void SpawnCubesForPlayer (Transform startPos, NetworkPlayer netPlayer, Color color) {
			cubeStartPoints.transform.rotation = startPos.rotation;
			cubeStartPoints.transform.position = startPos.position;
			var i = 1;
			foreach (Transform trans in cubeStartPoints.transform) {
				var cube = Instantiate (cubePrefab, trans.position, trans.rotation);
				if (Settings.Instance.AREnabled) {
					arManager.AddGameObjectToWorld (cube);
				}
				cube.GetComponent<Rigidbody> ().maxAngularVelocity = Settings.Instance.MaxRotationVelocity;
				cube.GetComponent<Colourer> ().color = color;
				cube.GetComponent<PlayerId> ().Id = netPlayer.PlayerNum;
				cube.name += i++;
				NetworkServer.SpawnWithClientAuthority (cube, netPlayer.PlayerGameObject);
			}
		}

		private Transform GetStartPosition () {
			return spawnPoints[nextSpawnPoint++ % spawnPoints.Length].transform;
		}
	}
}