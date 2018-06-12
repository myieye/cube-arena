using CubeArena.Assets.MyScripts.Logging.DAL.Models;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;

namespace CubeArena.Assets.MyScripts.Logging.DAL.Models {
    public class DeviceRoundConfig : BaseEntity {
        
        public int _deviceId;
        public UIMode _uiMode;
        public int _playerRoundId;
        

    }
}