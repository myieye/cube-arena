using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Utils.Constants {
    public static class MessageIds {
        public const short SetUIMode = MsgType.Highest + 1;
        public const short CustomHandleTransform_CA = MsgType.Highest + 2;
        public const short CustomHandleTransform_CA2 = MsgType.Highest + 3;
    }
}