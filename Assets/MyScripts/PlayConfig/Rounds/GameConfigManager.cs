using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.Logging.DAL;
using CubeArena.Assets.MyScripts.Logging.DAL.Models;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace CubeArena.Assets.MyScripts.PlayConfig.Rounds {

    public enum GameConfigMode { New, Old }

    public class GameConfigManager : MonoBehaviour {

        private const string NEW_CONFIG = "New Device Config";

        [SerializeField]
        private Dropdown gameConfigList;
        [SerializeField]
        private Dropdown roundList;

        public static GameConfigManager Instance { get; private set; }
        public GameConfigMode Mode {
            get {
                if (gameConfigList.value == 0) {
                    return GameConfigMode.New;
                } else {
                    return GameConfigMode.Old;
                }
            }
        }

        private List<GameConfig> gameConfigs;

        void Awake () {
#if !UNITY_EDITOR
            Destroy (gameConfigList.gameObject);
            Destroy (roundList.gameObject);
            Destroy (this);
#else
            Instance = this;
#endif
        }

        public void Refresh () {
            InitGameConfigList ();
        }

        public void OnSelectedGameConfigChanged () {
            roundList.gameObject.SetActive (gameConfigList.value > 0);
        }

        public List<List<PlayerRound>> GetSelectedGameConfig () {
            Assert.IsTrue (gameConfigList.value > 0);

            var gameId = gameConfigs[gameConfigList.value - 1].gameConfigId;
            return DataService.Instance.FindPlayerRoundsForGame (gameId);
        }

        public int GetStartingRound () {
            return roundList.value + 1;
        }

        private void InitGameConfigList () {
            gameConfigs = DataService.Instance.FindGameConfigs ();
            gameConfigList.options = (
                from gameConfig in gameConfigs select new Dropdown.OptionData (
                    string.Format ("ID: {0}. Players: {1}. Rounds: {2}. LastRound: _.",
                        gameConfig.gameConfigId, gameConfig.numPlayers, gameConfig.numRounds))).ToList ();

            gameConfigList.options.Insert (0, new Dropdown.OptionData (NEW_CONFIG));
            gameConfigList.value = 0;
            gameConfigList.RefreshShownValue();
            OnSelectedGameConfigChanged ();
        }

    }
}