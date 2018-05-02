using System;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils;
using UnityEngine;
using UnityEngine.Networking;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;

namespace CubeArena.Assets.MyScripts.Network {

    [DisallowMultipleComponent]
    [AddComponentMenu ("Network/RelativeNetworkTransform")]
    public class RelativeNetworkTransform : NetworkBehaviour {
        [SerializeField]
        private float interpolationRate;

        [SerializeField]
        private float positionThreshold = 1f;
        [SerializeField]
        private float rotationThreshold = 10f;
        [SerializeField]
        private float velocityThreshold = 3f;
        [SerializeField]
        private float angularVelocityThreshold = 3f;
        [SerializeField]
        private float interpolationSpeed = 0.4f;
        private const float MaxWait = 10f;

        private Rigidbody rb;
        private RigidbodyState rbs = new RigidbodyState ();
        private float wait = 0;

        void Awake () {
            rb = GetComponent<Rigidbody> ();
        }

        void Start () {
            TransformUtil.MoveToLocalCoordinates (transform);
        }

        void Update () {
            wait = Mathf.Lerp (wait, MaxWait, Time.deltaTime * interpolationSpeed);
            if (hasAuthority && PastThreshold ()) {
                TransmitSync ();
                SaveState ();
            }
        }

        [ClientCallback]
        void TransmitSync () {
            var relativeRbs = CalcStateInServerCoordinates ();
            if (rb) {
                CmdSyncRigidbody (relativeRbs);
            } else {
                CmdSyncTransform (relativeRbs);
            }
        }

        [Command]
        private void CmdSyncRigidbody (RigidbodyState rigidbodyState) {
            RpcBroadcastRigidbodyUpdate (rigidbodyState);
        }

        [Command]
        private void CmdSyncTransform (RigidbodyState rigidbodyState) {
            RpcBroadcastTransfromUpdate (rigidbodyState);
        }

        [ClientRpc]
        private void RpcBroadcastRigidbodyUpdate (RigidbodyState rigidbodyState) {
            if (hasAuthority || !rb) return;

            rigidbodyState = TransformToLocalCoordinates (rigidbodyState);

            rb.MovePosition (rigidbodyState.position);
            rb.MoveRotation (rigidbodyState.rotation);
            rb.velocity = rigidbodyState.velocity;
            rb.angularVelocity = rigidbodyState.angularVelocity;
        }

        [ClientRpc]
        private void RpcBroadcastTransfromUpdate (RigidbodyState rigidbodyState) {
            if (hasAuthority) return;

            rigidbodyState = TransformToLocalCoordinates (rigidbodyState);

            transform.position = rigidbodyState.position;
            transform.rotation = rigidbodyState.rotation;
        }

        private RigidbodyState CalcStateInServerCoordinates () {
            if (rb) {
                return TransformUtil.Transform (TransformDirection.LocalToServer, rb);
            } else {
                return TransformUtil.Transform (TransformDirection.LocalToServer, transform);
            }
        }

        private RigidbodyState TransformToLocalCoordinates (RigidbodyState rigidbodyState) {
            if (rb) {
                return TransformUtil.Transform (TransformDirection.ServerToLocal, ref rigidbodyState, true);
            } else {
                return TransformUtil.Transform (TransformDirection.ServerToLocal, ref rigidbodyState, false);
            }
        }

        private bool PastThreshold () {
            var pastThreshold =
                Vector3.Distance (transform.position, rbs.position) > ScaleThreshold (positionThreshold) ||
                Quaternion.Angle (transform.rotation, rbs.rotation) > ScaleThreshold (rotationThreshold);
            if (rb) {
                pastThreshold = pastThreshold ||
                    Vector3.Distance (rb.velocity, rbs.velocity) > ScaleThreshold (velocityThreshold) ||
                    Vector3.Distance (rb.angularVelocity, rbs.angularVelocity) > ScaleThreshold (angularVelocityThreshold);
            }
            return pastThreshold;
        }

        private void SaveState () {
            wait = 0;

            if (rb) {
                rbs.position = rb.position;
                rbs.rotation = rb.rotation;
                rbs.velocity = rb.velocity;
                rbs.angularVelocity = rb.angularVelocity;
            } else {
                rbs.position = transform.position;
                rbs.rotation = transform.rotation;
            }
        }

        private float ScaleThreshold (float threshold) {
            return threshold * (1 - (wait / MaxWait));
        }
    }
}