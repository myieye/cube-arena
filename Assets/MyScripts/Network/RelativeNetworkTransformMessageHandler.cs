using System;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Network {
    public static class RelativeNetworkTransformMessageHandler {

        private static Dictionary<NetworkInstanceId, NetworkWriter> serverTransformWriters;

        static RelativeNetworkTransformMessageHandler () {
            serverTransformWriters = new Dictionary<NetworkInstanceId, NetworkWriter> ();
        }

        public static void SendTransformToServer (NetworkWriter writer, RigidbodyState rigidbodyState,
            int messageId, NetworkInstanceId netId, bool sendVelocity) {
            NetworkMessageUtil.WriteRigidbodyState (writer, rigidbodyState,
                MessageIds.RelativeNetworkTransform_Server, messageId, netId, sendVelocity);
            ClientScene.readyConnection.SendWriter (writer, Channels.DefaultReliable);
        }

        public static void HandleRelativeNetworkTransform_Server (NetworkMessage netMsg) {
            NetworkInstanceId netId = netMsg.reader.ReadNetworkId ();

            CheckNetworkWriters (netId);

            GameObject foundObj;
            if (!(foundObj = NetworkServer.FindLocalObject (netId))) {
                Debug.LogError ("HandleRelativeNetworkTransform_Server: GameObject not found");
                return;
            }

            var serverTransformWriter = serverTransformWriters[netId];
            lock (serverTransformWriter) {
                NetworkMessageUtil.WriteRigidbodyStateReader (serverTransformWriter, netMsg.reader,
                    MessageIds.RelativeNetworkTransform_Client, netId);
                NetworkServer.SendWriterToReady (foundObj, serverTransformWriter, Channels.DefaultReliable);
            }
        }

        public static void HandleRelativeNetworkTransform_Client (NetworkMessage netMsg) {
            NetworkInstanceId netId = netMsg.reader.ReadNetworkId ();

            GameObject foundObj;
            if (!(foundObj = ClientScene.FindLocalObject (netId))) {
                Debug.LogError ("HandleRelativeNetworkTransform_Client: GameObject not found");
                return;
            }

            var messageId = netMsg.reader.ReadInt32 ();
            RigidbodyState rbs = NetworkMessageUtil.ReadRigidbodyState (netMsg.reader);
            foundObj.GetComponent<RelativeNetworkTransform> ().UpdatePosition (rbs, messageId);
        }

        private static void CheckNetworkWriters (NetworkInstanceId netId) {
            lock (serverTransformWriters) {
                if (!serverTransformWriters.ContainsKey (netId)) {
                    serverTransformWriters.Add (netId, new NetworkWriter ());
                }
            }
        }
    }
}