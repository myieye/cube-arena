    using System.Collections.Generic;
    using System.Linq;
    using CubeArena.Assets.MyScripts.Logging.DAL.Models;
    using CubeArena.Assets.MyScripts.Logging.DAL;
    using UnityEngine.Assertions;
    using UnityEngine.UI;
    using UnityEngine;

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
#if UNITY_EDITOR || UNITY_STANDALONE
                    if (gameConfigList.value == 0) {
                        return GameConfigMode.New;
                    } else {
                        return GameConfigMode.Old;
                    }
#else
                    return GameConfigMode.New;
#endif
                }
            }

            private List<GameConfig> gameConfigs;

            void Awake () {
#if !(UNITY_EDITOR || UNITY_STANDALONE)
                Destroy (gameConfigList.gameObject);
                Destroy (roundList.gameObject);
#endif
                Instance = this;
            }

            public void Refresh () {
                InitGameConfigList ();
            }

            public void OnSelectedGameConfigChanged () {
#if UNITY_EDITOR || UNITY_STANDALONE
                roundList.gameObject.SetActive (gameConfigList.value > 0);
#endif
            }

            public List<List<PlayerRound>> GetSelectedGameConfig () {
                Assert.IsTrue (gameConfigList.value > 0);

                var gameId = gameConfigs[gameConfigList.value - 1].Id;
                return DataService.Instance.FindPlayerRoundsForGame (gameId);
            }

            public int GetStartingRound () {
                return roundList.value + 1;
            }

            private void InitGameConfigList () {
#if UNITY_EDITOR || UNITY_STANDALONE
                gameConfigs = DataService.Instance.FindGameConfigs ();
                gameConfigList.options = (
                    from gameConfig in gameConfigs select new Dropdown.OptionData (
                        ToGameConfigString (gameConfig))).ToList ();

                gameConfigList.options.Insert (0, new Dropdown.OptionData (NEW_CONFIG));
                gameConfigList.value = 0;
                gameConfigList.RefreshShownValue ();
                OnSelectedGameConfigChanged ();
#endif
            }

            private string ToGameConfigString (GameConfig gameConfig) {
                return string.Format ("ID: {0}. Players: {1}. Rounds: {2}. Created: {3}.",
                    gameConfig.Id, gameConfig.NumberOfPlayers,
                    gameConfig.NumberOfRounds, gameConfig.Created);
            }

        }
    }