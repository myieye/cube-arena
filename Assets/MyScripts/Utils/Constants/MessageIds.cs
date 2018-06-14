using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Utils.Constants {
    public static class MessageIds {
        public const short SetUIMode = MsgType.Highest + 1;
        public const short CustomHandleTransform_CA = MsgType.Highest + 2;
        public const short CustomHandleTransform_CA2 = MsgType.Highest + 3;
        public const short RelativeNetworkTransform_Server = MsgType.Highest + 4;
        public const short RelativeNetworkTransform_Client = MsgType.Highest + 5;
    }
}