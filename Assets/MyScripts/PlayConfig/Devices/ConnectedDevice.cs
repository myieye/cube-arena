using CubeArena.Assets.MyScripts.Logging.DAL.Models;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.PlayConfig.Devices {
    public class ConnectedDevice : Device {

        public ConnectedDevice (NetworkConnection connection, short controllerId, DeviceTypeSpec type, string model) {
            this.Connection = connection;
            this.ControllerId = controllerId;
            this.Type = type;
            this.Model = model;
        }

        public NetworkConnection Connection { get; set; }
        public short ControllerId { get; set; }
    }
}