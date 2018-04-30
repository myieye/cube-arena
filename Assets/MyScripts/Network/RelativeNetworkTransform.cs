using System;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Network {

    [DisallowMultipleComponent]
    [AddComponentMenu ("Network/RelativeNetworkTransform")]
    public class RelativeNetworkTransform : NetworkBehaviour {
        [SerializeField]
        private float interpolationRate;
        [SerializeField]
        private Transform origin;

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

        private bool updated;
        private Rigidbody rb;
        private RigidbodyState rbs = new RigidbodyState ();
        private float wait = 0;

        void Start () {
            rb = GetComponent<Rigidbody> ();
            if (!origin) {
                origin = GameObject.Find(Names.ARWorld).transform;
            }
        }

        void Update () {
            wait = Mathf.Lerp(wait, MaxWait, Time.deltaTime * interpolationSpeed);
            if (hasAuthority && PastThreshold ()) {
                TransmitSync ();
                SaveState ();
            }
        }

        [ClientCallback]
        void TransmitSync () {
            var relativeRbs = CalcStateRelativeToOrigin();
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
            if (hasAuthority) return;

            if (origin && !IsCentered(origin)) {
                rigidbodyState = TransformFromStateRelativeToOrigin(rigidbodyState);
            }
            rb.MovePosition(rigidbodyState.position);
            rb.MoveRotation(rigidbodyState.rotation);
            rb.velocity = rigidbodyState.velocity;
            rb.angularVelocity = rigidbodyState.angularVelocity;
        }

        [ClientRpc]
        private void RpcBroadcastTransfromUpdate (RigidbodyState rigidbodyState) {
            if (hasAuthority) return;

            if (origin && !IsCentered(origin)) {
                rigidbodyState = TransformFromStateRelativeToOrigin(rigidbodyState);
            }
            transform.position = rigidbodyState.position;
            transform.rotation = rigidbodyState.rotation;
        }

        private RigidbodyState CalcStateRelativeToOrigin () {
            var relativeRbs = new RigidbodyState {
                position = transform.position - origin.transform.position,
                rotation = transform.rotation * Quaternion.Inverse(origin.transform.rotation),
            };

            if (rb) {
                var rotation = transform.rotation * Quaternion.Inverse(relativeRbs.rotation);
                relativeRbs.velocity = rotation * rb.velocity;
                relativeRbs.angularVelocity = rotation * rb.angularVelocity;
            }

            return relativeRbs;
        }

        private RigidbodyState TransformFromStateRelativeToOrigin (RigidbodyState rigidbodyState) {
            var rbs = new RigidbodyState {
                position = rigidbodyState.position + origin.transform.position,
                rotation = rigidbodyState.rotation * origin.transform.rotation
            };

            if (rb) {
                var rotation = transform.rotation * Quaternion.Inverse(rbs.rotation);
                rbs.velocity = rotation * rb.velocity;
                rbs.angularVelocity = rotation * rb.angularVelocity;
            }
            return rbs;
        }

        private bool PastThreshold () {
            var pastThreshold = 
                Vector3.Distance (transform.position, rbs.position) > ScaleThreshold(positionThreshold) ||
                Quaternion.Angle (transform.rotation, rbs.rotation) > ScaleThreshold(rotationThreshold);
            if (rb) {
                pastThreshold = pastThreshold ||
                    Vector3.Distance (rb.velocity, rbs.velocity) > ScaleThreshold(velocityThreshold) ||
                    Vector3.Distance (rb.angularVelocity, rbs.angularVelocity) > ScaleThreshold(angularVelocityThreshold);
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

        private float ScaleThreshold(float threshold) {
            return threshold * (1 - (wait / MaxWait));
        }

        private bool IsCentered(Transform transform) {
            return transform.position == Vector3.zero &&
                transform.rotation == Quaternion.identity;
        }
    }
}