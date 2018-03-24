using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Agents;
using CubeArena.Assets.MyScripts.AR;
using CubeArena.Assets.MyScripts.Constants;
using CubeArena.Assets.MyScripts.Rounds;
using CubeArena.Assets.MyScripts.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Network
{
	public class CustomNetworkManager : NetworkManager {

		public Transform spawnPoints;
		public GameObject cubeStartPoints;
		public GameObject cubePrefab;
		public EnemyManager enemyManager;
		public RoundManager roundManager;
		public Settings settings;
		public ARManager arManager;
		public Material[] materials;
		//private Dictionary<NetworkConnection, int> connectedPlayers = new Dictionary<NetworkConnection, int>();
		private Dictionary<string, NetworkPlayer> networkPlayers = new Dictionary<string, NetworkPlayer>();

		override public void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
			Debug.Log("OnServerAddPlayer");
			var key = GenerateConnectionKey(conn, playerControllerId);
			if (!networkPlayers.ContainsKey(key)) {
				var netPlayer = new NetworkPlayer {
					Connection = conn,
					PlayerControllerId = playerControllerId,
					PlayerId = numPlayers + 1 };
				networkPlayers[key] = netPlayer;
				SpawnPlayer(netPlayer);
			}
			//Debug.Log("AddPlayer: " + Network.connections.Length);
			//GetComponent<NetworkIdentity>().netId.Value
		}

		public override void OnServerSceneChanged(string sceneName) {
			CheckStartPositions();
			if (roundManager.OnSceneLoaded()) {
				foreach (var netPlayer in networkPlayers.Values) {
					SpawnPlayer(netPlayer);
				}
			}
		}

		private void SpawnPlayer(NetworkPlayer netPlayer) {
			var startPos = GetStartPosition();
			var color = materials[netPlayer.PlayerId - 1].color;
			netPlayer.Player = SpawnPlayerCursor(netPlayer, color);
			SpawnCubesForPlayer(startPos, netPlayer, color);
			enemyManager.OnPlayerAdded();
		}

        GameObject SpawnPlayerCursor(NetworkPlayer netPlayer, Color color) {
			var player = (GameObject) GameObject.Instantiate(playerPrefab);
			if (settings.AREnabled) {
				arManager.AddARObjectToWorld(player.GetComponent<ARObject>());
			}
			color = Highlight.ReduceTransparency(color, Highlight.CursorTransparency);
			player.GetComponent<Colourer>().color = color;
			player.GetComponent<PlayerId>().Id = netPlayer.PlayerId;
			NetworkServer.AddPlayerForConnection(netPlayer.Connection, player, netPlayer.PlayerControllerId);
			return player;
		}

		void SpawnCubesForPlayer(Transform startPos, NetworkPlayer netPlayer, Color color) {
			cubeStartPoints.transform.rotation = startPos.rotation;
			cubeStartPoints.transform.position = startPos.position;

			foreach (Transform trans in cubeStartPoints.transform) {
				var cube = Instantiate(cubePrefab, trans.position, trans.rotation);
				if (settings.AREnabled) {
					arManager.AddGameObjectToWorld(cube);
				}
				cube.GetComponent<Colourer>().color = color;
				cube.GetComponent<PlayerId>().Id = netPlayer.PlayerId;
				NetworkServer.SpawnWithClientAuthority(cube, netPlayer.Player);
			}
		}

		private void CheckStartPositions() {
			if (startPositions == null || startPositions.Count == 0) {
				foreach (Transform startPos in spawnPoints) {
					RegisterStartPosition(startPos);
				}
			}
		}

		private string GenerateConnectionKey(NetworkConnection conn, short playerControllerId) {
			return string.Format("{0}:{1}:{2}", conn.connectionId, conn.address, playerControllerId);
		}
	}
}