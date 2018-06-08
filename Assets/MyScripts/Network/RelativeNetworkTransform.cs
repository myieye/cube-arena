using System;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Attributes;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Network {

    [DisallowMultipleComponent]
    [AddComponentMenu ("Network/RelativeNetworkTransform")]
    public class RelativeNetworkTransform : NetworkBehaviour {

        [SerializeField]
        private bool sendUpdates = true;
        [SerializeField]
        [ConditionalField ("sendUpdates", true)]
        private float movementInterpolation;
        [SerializeField]
        [ConditionalField ("sendUpdates", true)]
        private float rotationInterpolation;
        [ConditionalField ("sendUpdates", true)]
        [SerializeField]
        private float positionThreshold = 1f;
        [SerializeField]
        [ConditionalField ("sendUpdates", true)]
        private float rotationThreshold = 10f;
        [SerializeField]
        [ConditionalField ("sendUpdates", true)]
        private float velocityThreshold = 3f;
        [SerializeField]
        [ConditionalField ("sendUpdates", true)]
        private float angularVelocityThreshold = 3f;
        [SerializeField]
        [ConditionalField ("sendUpdates", true)]
        private float interpolationSpeed = 0.4f;
        private const float MaxWait = 10f;
        private int navMeshMissCount;
        private NetworkTransformMode mode;
        private Rigidbody rb;
        private NavMeshAgent agent;
        private RigidbodyState rbs = new RigidbodyState ();
        private RigidbodyState targetState = new RigidbodyState ();
        private float wait = 0;
        protected bool isInitialized;
        public bool IsSender {
            get {
                return hasAuthority || (!localPlayerAuthority && isServer);
            }
        }
        private Vector3 startPosition;
        private Quaternion startRotation;

        private long nextMessageId = 0;
        private long prevMessageId = -1;

        protected virtual void Awake () {
            agent = GetComponent<NavMeshAgent> ();
            rb = GetComponent<Rigidbody> ();

            startPosition = transform.position;
            startRotation = transform.rotation;

            if (agent) {
                mode = NetworkTransformMode.Agent;
                agent.enabled = false;
            } else if (rb) {
                mode = NetworkTransformMode.Rigidbody;
            } else {
                mode = NetworkTransformMode.Transform;
            }
        }

        protected virtual void Start () {
            InvokeRepeating ("TryInit", 0, 0.1f);
        }

        private void TryInit () {
            if (Init ()) {
                CancelInvoke ("TryInit");
            }
        }

        protected bool Init () {
            if (!TransformUtil.IsInitialized) return false;

            switch (mode) {
                case NetworkTransformMode.Agent:
                    //agent.enabled = false;
                    agent.Warp (startPosition.ToLocal ());
                    transform.rotation = startRotation.ToLocal ();
                    agent.enabled = true;
                    break;
                case NetworkTransformMode.Rigidbody:
                    targetState = rb.ToLocalState ();
                    transform.MoveToLocal ();
                    break;
                default:
                    targetState = transform.ToLocalState ();
                    transform.MoveToLocal ();
                    break;
            }

            return isInitialized = true;
        }

        protected virtual void Update () {
            if (!IsSender || !sendUpdates) return;

            //wait = Mathf.Lerp (wait, MaxWait, Time.deltaTime * interpolationSpeed);
            //if (PastThreshold ()) {
            TransmitSync ();
            //    SaveState ();
            //}
        }

        protected virtual void FixedUpdate () {
            if (!sendUpdates || IsSender || !isInitialized) {
                return;
            }

            switch (mode) {
                case NetworkTransformMode.Rigidbody:
                    if (!rb) return;
                    rb.MovePosition (Vector3.Lerp (rb.position, targetState.position, movementInterpolation));
                    rb.MoveRotation (Quaternion.Lerp (rb.rotation, targetState.rotation, rotationInterpolation));
                    rb.velocity = Vector3.Lerp (rb.velocity, targetState.velocity, movementInterpolation);
                    rb.angularVelocity = Vector3.Lerp (rb.angularVelocity, targetState.angularVelocity, rotationInterpolation);
                    break;
                case NetworkTransformMode.Transform:
                    transform.position = Vector3.Lerp (transform.position, targetState.position, movementInterpolation);
                    transform.rotation = Quaternion.Lerp (transform.rotation, targetState.rotation, rotationInterpolation);
                    break;
            }
        }

        void TransmitSync () {
            var relativeRbs = CalcStateInServerCoordinates ();
            CmdSyncPosition (relativeRbs, nextMessageId);
        }

        [Command]
        private void CmdSyncPosition (RigidbodyState rigidbodyState, long messageId) {
            RpcBroadcastPosition (rigidbodyState, messageId);
        }

        [ClientRpc]
        private void RpcBroadcastPosition (RigidbodyState rigidbodyState, long messageId) {
            if (IsSender || !isInitialized || messageId < prevMessageId) return;

            prevMessageId = messageId;
            rigidbodyState = TransformToLocalCoordinates (rigidbodyState);
            if (!IsValid (rigidbodyState)) return;
            targetState = rigidbodyState;

            switch (mode) {
                /*case NetworkTransformMode.Rigidbody:
                    if (!rb) return;
                    rb.MovePosition (rigidbodyState.position);
                    rb.MoveRotation (rigidbodyState.rotation);
                    rb.velocity = rigidbodyState.velocity;
                    rb.angularVelocity = rigidbodyState.angularVelocity;
                    break;
                case NetworkTransformMode.Transform:
                    transform.position = rigidbodyState.position;
                    transform.rotation = rigidbodyState.rotation;
                    break;*/
                case NetworkTransformMode.Agent:
                    if (agent.isOnNavMesh) {
                        navMeshMissCount = 0;
                        agent.Move (rigidbodyState.position - transform.position);
                        agent.transform.rotation = rigidbodyState.rotation;
                    } else {
                        navMeshMissCount++;
                        if (navMeshMissCount > 5) {
                            agent.Warp (rigidbodyState.position);
                        }
                    }
                    break;
            }
        }

        private bool IsValid (RigidbodyState rigidbodyState) {
            var isValid = TransformUtil.IsValid (rigidbodyState.position) &&
                TransformUtil.IsValid (rigidbodyState.rotation);

            if (mode == NetworkTransformMode.Rigidbody) {
                isValid = isValid && TransformUtil.IsValid (rigidbodyState.velocity) &&
                    TransformUtil.IsValid (rigidbodyState.angularVelocity);
            }

            return isValid;
        }

        private RigidbodyState CalcStateInServerCoordinates () {
            if (mode == NetworkTransformMode.Rigidbody) {
                return rb.ToServerState ();
            } else {
                return transform.ToServerState ();
            }
        }

        private RigidbodyState TransformToLocalCoordinates (RigidbodyState rigidbodyState) {
            if (mode == NetworkTransformMode.Rigidbody) {
                return TransformUtil.Transform (TransformDirection.ServerToLocal, ref rigidbodyState, true);
            } else {
                return TransformUtil.Transform (TransformDirection.ServerToLocal, ref rigidbodyState, false);
            }
        }

        private bool PastThreshold () {
            var pastThreshold =
                Vector3.Distance (transform.position, rbs.position) > ScaleThreshold (positionThreshold) ||
                Quaternion.Angle (transform.rotation, rbs.rotation) > ScaleThreshold (rotationThreshold);
            if (mode == NetworkTransformMode.Rigidbody) {
                if (!rb) return false;
                pastThreshold = pastThreshold ||
                    Vector3.Distance (rb.velocity, rbs.velocity) > ScaleThreshold (velocityThreshold) ||
                    Vector3.Distance (rb.angularVelocity, rbs.angularVelocity) > ScaleThreshold (angularVelocityThreshold);
            }
            return pastThreshold;
        }

        private void SaveState () {
            wait = 0;
            if (mode == NetworkTransformMode.Rigidbody) {
                if (!rb) return;
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