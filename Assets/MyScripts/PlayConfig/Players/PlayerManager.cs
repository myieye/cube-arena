using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.Logging;
using CubeArena.Assets.MyScripts.Logging.DAL;
using CubeArena.Assets.MyScripts.Logging.DAL.Models;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using CubeArena.Assets.MyScripts.PlayConfig.Rounds;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Helpers;
using CubeArena.Assets.MyScripts.Utils.Settings;
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

        public List<NetworkPlayer> Players { get; private set; }
        public int NumberOfActivePlayers { get { return Players != null ? Players.Count : 0; } }
        public int NumberOfPlayersForRound {
            get {
                switch (Settings.Instance.PlayerNumberMode) {
                    case PlayerNumberMode.NumberOfDevices:
                        return DeviceManager.Instance.ConnectedDevices.Count;
                    case PlayerNumberMode.NumberOfTestDevices:
                        return DeviceManager.Instance.ConnectedDevices.Where (
                            device => device.Value.Type.IsTestDeviceType ()).Count ();
                    case PlayerNumberMode.Custom:
                    default:
                        return Settings.Instance.NumberOfPlayers;
                }
            }
        }

        public int MaxPlayers { get { return PlayerSpawner.Instance.colors.Length; } }

        private List<List<int>> gamePlayerRoundIds;
        private List<int> playerRoundIds;
        private DataService dataService;

        void Start () {
            dataService = DataService.Instance;
            Players = new List<NetworkPlayer> ();
        }

        public int GenerateNewPlayers () {
            DestroyPlayers ();

            Players = new List<NetworkPlayer> ();
            for (int i = 0; i < NumberOfPlayersForRound; i++) {
                Players.Add (new NetworkPlayer {
                    PlayerId = dataService.GetNextPlayerId (),
                        PlayerIndex = i, PlayerNum = i + 1
                });
            }
            return NumberOfPlayersForRound;
        }

        public void InitPlayersWithPlayerRounds (List<List<PlayerRound>> playerRounds) {
            DestroyPlayers ();

            Players = new List<NetworkPlayer> ();
            foreach (var playerRound in playerRounds.First ()) {
                Players.Add (new NetworkPlayer {
                    PlayerId = playerRound.PlayerId,
                        PlayerIndex = playerRound.PlayerNum - 1,
                        PlayerNum = playerRound.PlayerNum
                });
            }

            gamePlayerRoundIds = (from playerRound in playerRounds select (
                from player in playerRound select player.Id).ToList ()).ToList ();
        }

        public void DestroyPlayers () {
            if (Players != null && Players.Any ()) {
                foreach (var player in Players) {
                    NetworkServer.Destroy (player.PlayerGameObject);
                }
            }
        }

        public void GeneratePlayerRounds (List<List<DeviceConfig>> deviceRoundConfigs) {
            Assert.AreEqual (Players.Count, deviceRoundConfigs[0].Count);

            gamePlayerRoundIds = dataService.CreatePlayerRounds (Players, deviceRoundConfigs);
            GameConfigManager.Instance.Refresh ();
        }

        public List<NetworkPlayer> ConfigurePlayersForRound (int roundNum, List<DeviceConfig> deviceRoundConfig) {
            Assert.AreEqual (Players.Count, deviceRoundConfig.Count);

            for (int i = 0; i < Players.Count; i++) {
                Players[i].DeviceConfig = deviceRoundConfig[i];
            }

            playerRoundIds = gamePlayerRoundIds[roundNum - 1];
            return Players;
        }

        public int GetPlayerRoundId (int playerId) {
            return playerRoundIds != null ?
                playerRoundIds[Players.FindIndex (p => p.PlayerId == playerId)] : -1;
        }

        public Color GetPlayerColor (PlayerId id) {
            return FindPlayer (id).Color.value;
        }

        public List<GameObject> GetPlayerCubes (PlayerId id) {
            return FindPlayer (id).Cubes;
        }

        public NetworkConnection GetPlayerConnection (PlayerId playerId) {
            return FindPlayer (playerId).DeviceConfig.Device.Connection;
        }

        public Measure GetPlayerMeasurer (PlayerId playerId) {
            return FindPlayer (playerId).PlayerGameObject.GetComponent<Measure> ();
        }

        public void SpawnPlayers () {
            PlayerSpawner.Instance.SpawnPlayers (Players);
        }

        public void ClearPlayers (float time) {
            foreach (var netPlayer in Players) {
                if (netPlayer.Cursor) {
                    var measure = netPlayer.Cursor.GetComponent<Measure> ();
                    if (measure) {
                        measure.RpcFlushMeasurements ();
                    }
                }
                // TODO: delay is likely causing Rpc object not found warning.
                StartCoroutine (DelayUtil.Do (time - 0.2f, () => NetworkServer.Destroy (netPlayer.Cursor)));
                if (netPlayer.Cubes != null) {
                    netPlayer.Cubes.ForEach (cube => NetworkServer.Destroy (cube));
                }
            }
        }

        private NetworkPlayer FindPlayer (PlayerId playerId) {
            return Players.First (player => player.PlayerId == playerId.Id);
        }
    }
}