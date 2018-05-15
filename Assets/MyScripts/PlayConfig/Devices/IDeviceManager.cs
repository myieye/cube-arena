using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace CubeArena.Assets.MyScripts.PlayConfig.Devices {

    public interface IDeviceManager {
        Dictionary<string, ConnectedDevice> ConnectedDevices { get; }
        Dictionary<DeviceTypeSpec, List<ConnectedDevice>> DevicesByType { get; }
        void RegisterConnectedDevice (ConnectedDevice connectedDevice);
        void UnregisterDevice (NetworkConnection conn);
        void ResetDevices ();
        bool HasConnectedDevice (NetworkConnection conn, short controllerId);
        bool EnoughDevicesAvailable (int numPlayers);
    }
}