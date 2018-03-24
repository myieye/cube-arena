namespace CubeArena.Assets.MyScripts.UI.Mode
{
    public enum UIMode {
        Mouse = 1
        ,HHD1_Camera = 2
        ,HHD2_TouchAndDrag = 3
        ,HHD3_Gestures = 4
        #if (UNITY_WSA || UNITY_EDITOR)
        ,HMD4_GazeAndClicker = 5
        ,HMD5_Gaze__AirTap_Drag_And_Clicker_Rotate = 6
        #endif
    }

    static class UIModeMethods
    {

        public static string GetFriendlyString(this UIMode uiMode)
        {
            switch (uiMode)
            {
                default:
                    return uiMode.ToString()
                        .Replace("__", ", ")
                        .Replace("_", " ")
                        .Replace("And", " + ");
            }
        }
    }
}