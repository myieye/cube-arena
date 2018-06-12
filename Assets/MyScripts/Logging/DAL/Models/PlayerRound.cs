using CubeArena.Assets.MyScripts.Logging.DAL.SQLite;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;

namespace CubeArena.Assets.MyScripts.Logging.DAL.Models {
    public class PlayerRound : BaseEntity {

        public int _gameConfigId;
        public int _playerId;
        public int _playerNum;
        public int _roundNum;
        public UIMode _ui;
        public int _deviceId;
        

        [NotNull]
        public int GameConfigId {
            get { return _gameConfigId; }
            set { _gameConfigId = value; }
        }
        
        [NotNull]
        public int PlayerId {
            get { return _playerId; }
            set { _playerId = value; }
        }
        
        [NotNull]
        public int PlayerNum {
            get { return _playerNum; }
            set { _playerNum = value; }
        }

        [NotNull]
        public int RoundNum {
            get { return _roundNum; }
            set { _roundNum = value; }
        }

        [NotNull]
        public UIMode UI {
            get { return _ui; }
            set { _ui = value; }
        }

        [NotNull]
        public int DeviceId
        {
            get { return _deviceId;}
            set { _deviceId = value;}
        }

        public override string ToString () {
            return string.Format (
                "PlayerRound: [{0}. GameConfigId: {1}. PlayerId: {2}. RoundNum: {3}. UI: {4}.]",
                base.ToString (), GameConfigId, PlayerId, RoundNum, UI);
        }
    }
}