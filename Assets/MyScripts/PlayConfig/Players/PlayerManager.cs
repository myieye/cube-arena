using System;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.Logging.DAL;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.PlayConfig.Players {
    public class PlayerManager : NetworkBehaviour {

        public static PlayerManager Instance { get; private set; }
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
            if (Instance) {
                Destroy (this);
            } else {
                Instance = this;
                dataService = DataService.Instance;
                players = new List<NetworkPlayer> ();
            }
        }

        public int GenerateNewPlayers () {
            players = new List<NetworkPlayer> ();
            for (int i = 0; i < NumPlayers; i++) {
                players.Add (new NetworkPlayer {
                    PlayerId = dataService.GetNextPlayerId (),
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

        internal void SpawnPlayers () {
            PlayerSpawner.Instance.SpawnPlayers(players);
        }
    }
}