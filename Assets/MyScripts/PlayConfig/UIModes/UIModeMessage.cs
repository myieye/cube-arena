using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.PlayConfig.UIModes {
    public class UIModeMessage : MessageBase {
        public UIMode UIMode;
        public int PlayerNum;
        public float PassToPlayerTime;
        public bool DisableUIModeList;
        public bool ForceUserStudySettings;
    }
}