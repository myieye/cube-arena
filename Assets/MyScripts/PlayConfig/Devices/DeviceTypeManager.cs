namespace CubeArena.Assets.MyScripts.PlayConfig.Devices {
    public static class DeviceTypeManager {

        public static DeviceTypeSpec DeviceType { get; private set; }

        static DeviceTypeManager () {
            DeviceType =
#if UNITY_EDITOR
                DeviceTypeSpec.Desktop;
#elif UNITY_ANDROID
            DeviceTypeSpec.Android;
#elif UNITY_WSA
            DeviceTypeSpec.HoloLens;
#else
            DeviceTypeSpec.Desktop;
#endif
        }

        public static bool IsDeviceType (DeviceTypeSpec deviceType) {
            return DeviceType == deviceType;
        }

    }
}