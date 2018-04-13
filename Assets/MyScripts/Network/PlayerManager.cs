using System;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Data;
using CubeArena.Assets.MyScripts.Network;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Network {
    public class PlayerManager : NetworkBehaviour {

        public static PlayerManager Instance { get; private set; }
        private DataService dataService;
        private List<NetworkPlayer> players;
        private Dictionary<int, int> playerRoundIdByPlayerId;
        
        void Start () {
            if (Instance) {
                Destroy(this);
            } else {
                Instance = this;
                dataService = DataService.Instance;
                players = new List<NetworkPlayer> ();
            }
        }

        public void GeneratePlayerRoundIds (int roundNum) {
            var playerRoundIds = dataService.CreatePlayerRoundIds(
                from netPlayer in players select netPlayer.PlayerId,
                roundNum);
            
            playerRoundIdByPlayerId = new Dictionary<int, int>();
            for (var i = 0; i < playerRoundIds.Count(); i ++) {
                playerRoundIdByPlayerId.Add(players[i].PlayerId, playerRoundIds[i]);
            }
        }

        public NetworkPlayer AddPlayer (NetworkConnection conn, short playerControllerId) {
            var player = new NetworkPlayer {
                Connection = conn,
                PlayerControllerId = playerControllerId,
                PlayerNum = players.Count + 1,
                PlayerId = dataService.GetNextPlayerId()
            };
            players.Add(player);
            return player;
        }

        public int GetPlayerRoundId(int playerId) {
            return playerRoundIdByPlayerId != null ? playerRoundIdByPlayerId[playerId] : -1;
        }
    }
}