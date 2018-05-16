using CubeArena.Assets.MyScripts.PlayConfig.Players;

namespace CubeArena.Assets.MyScripts.Utils.Constants {
    public static class Text {
        public static string Spray { get; private set; }
        public static string Move { get; private set; }

        static Text () {
            Spray = string.Format ("{0} {1}", ActiveText ("Spray"), InactiveText ("Move"));
            Move = string.Format ("{0} {1}", InactiveText ("Spray"), ActiveText ("Move"));
        }

        public static string PassToPlayerText (int playerNum) {
            return string.Format ("Pass device to player: <size=60>{0}</size>", playerNum);
        }

        public static string CubeName (NetworkPlayer player, int cubeNum) {
            return string.Format ("Cube [{0}:{1}:{2}]", player.PlayerId, player.Color.name, cubeNum);
        }

        public static string CursorName (NetworkPlayer player) {
            return string.Format ("Cursor [{0}:{1}]", player.PlayerId, player.Color.name);
        }

        private static string InactiveText (string text) {
            return string.Format ("<size=20><color=#CCC>{0}</color></size>", text);
        }

        private static string ActiveText (string text) {
            return string.Format ("<b>{0}</b>", text);
        }
    }
}