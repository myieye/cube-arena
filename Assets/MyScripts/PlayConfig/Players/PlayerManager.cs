using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.Logging.DAL;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.PlayConfig.Players {
    public class PlayerManager : NetworkBehaviourSingleton {

        public static PlayerManager Instance {
            get {
                return Instance<PlayerManager> ();
            }
        }
        private List<NetworkPlayer> players;
        public int NumPlayers {
            get {
                return numPlayers;
            }
        }

        [SerializeField]
        private int numPlayers;
        private List<int> playerRoundIds;
        private DataService dataService;

        void Start () {
            dataService = DataService.Instance;
            players = new List<NetworkPlayer> ();
        }

        public int GenerateNewPlayers () {
            numPlayers = DeviceManager.Instance<DeviceManager> ().ConnectedDevices.Count;
            players = new List<NetworkPlayer> ();
            for (int i = 0; i < NumPlayers; i++) {
                players.Add (new NetworkPlayer {
                    PlayerId = dataService.GetNextPlayerId (),
                        Color = PlayerSpawner.Instance.materials[i].color,
                        PlayerNum = i + 1
                });
            }
            return NumPlayers;
        }

        public List<NetworkPlayer> ConfigurePlayersForRound (int roundNum, List<DeviceConfig> deviceRoundConfig) {
            Assert.AreEqual (players.Count, deviceRoundConfig.Count);

            for (int i = 0; i < players.Count; i++) {
                players[i].DeviceConfig = deviceRoundConfig[i];
            }

            playerRoundIds = dataService.CreatePlayerRoundIds (players, roundNum);
            return players;
        }

        public int GetPlayerRoundId (int playerId) {
            return playerRoundIds != null ?
                playerRoundIds[players.FindIndex (p => p.PlayerId == playerId)] : -1;
        }

        public Color GetPlayerColor (PlayerId id) {
            return players.Find (p => p.PlayerId == id.Id).Color;
        }

        public void SpawnPlayers () {
            PlayerSpawner.Instance.SpawnPlayers (players);
        }

        public void ResetPlayers () {
            foreach (var cube in GameObject.FindGameObjectsWithTag(Tags.Cube)) {
                NetworkServer.Destroy(cube);
            }
        }
    }
}