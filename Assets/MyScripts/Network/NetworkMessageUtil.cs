using System;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Network {
    public static class NetworkMessageUtil {

        public static void WriteRigidbodyState (NetworkWriter writer, RigidbodyState rbs,
            short msgType, int messageId,
            NetworkInstanceId netId, bool withVelocity) {
            writer.StartMessage (msgType);
            writer.Write (netId);
            writer.Write (messageId);
            writer.Write (withVelocity);
            writer.Write (rbs.position);
            writer.Write (rbs.rotation);
            if (withVelocity) {
                writer.Write (rbs.velocity);
                writer.Write (rbs.angularVelocity);
            }
            writer.FinishMessage ();
        }

        public static void WriteRigidbodyStateReader (NetworkWriter writer, NetworkReader reader,
            short msgType, NetworkInstanceId netId) {
            writer.StartMessage (msgType);
            writer.Write (netId);
            writer.Write (reader.ReadInt32 ());
            var withVelocity = reader.ReadBoolean ();
            writer.Write (withVelocity);
            writer.Write (reader.ReadVector3 ());
            writer.Write (reader.ReadQuaternion ());
            if (withVelocity) {
                writer.Write (reader.ReadVector3 ());
                writer.Write (reader.ReadVector3 ());
            }
            writer.FinishMessage ();
        }

        public static RigidbodyState ReadRigidbodyState (NetworkReader reader) {
            var isRigidbody = reader.ReadBoolean ();
            var rbs = new RigidbodyState () {
                position = reader.ReadVector3 (),
                rotation = reader.ReadQuaternion ()
            };
            if (isRigidbody) {
                rbs.velocity = reader.ReadVector3 ();
                rbs.angularVelocity = reader.ReadVector3 ();
            }
            return rbs;
        }
    }
}