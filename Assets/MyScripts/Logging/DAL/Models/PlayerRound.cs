using CubeArena.Assets.MyScripts.Logging.DAL.SQLite;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;

namespace CubeArena.Assets.MyScripts.Logging.DAL.Models {
    public class PlayerRound : BaseEntity {

        public int _playerId;
        public int _roundNum;
        public UIMode _ui;
        public int _deviceId;
        

        [NotNull]
        public int PlayerId {
            get { return _playerId; }
            set { _playerId = value; }
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
        public int DeviceId
        {
            get { return _deviceId;}
            set { _deviceId = value;}
        }
        

        public override string ToString () {
            return string.Format (
                "PlayerRound: [{0}. PlayerId: {1}. RoundNum: {2}. UI: {3}.]",
                base.ToString (), PlayerId, RoundNum, UI);
        }
    }
}