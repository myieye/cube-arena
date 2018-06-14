using System;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.AR;
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
        private float positionThreshold = 0.01f;
        [SerializeField]
        [ConditionalField ("sendUpdates", true)]
        private float rotationThreshold = 1f;
        [SerializeField]
        [ConditionalField ("sendUpdates", true)]
        private float maxWait = 2;

        // Mode ---
        private NetworkTransformMode mode;
        public bool SyncVelocity { get { return mode == NetworkTransformMode.Rigidbody; } }
        // ---

        // State ---
        protected bool isInitialized;
        private Vector3 startPosition;
        private Quaternion startRotation;

        private Rigidbody rb;
        private RigidbodyState savedRbs = new RigidbodyState ();
        private RigidbodyState localTargetState = new RigidbodyState ();
        private RigidbodyState serverTargetState = new RigidbodyState ();
        
        private NavMeshAgent agent;
        private int navMeshMissCount;
        // ---

        // Network ---
        public bool IsSender {
            get {
                return hasAuthority || (!localPlayerAuthority && isServer);
            }
        }
        private bool wasSenderInLastFrame;
        private float wait = 0;
        private int nextMessageId = 1;
        private int prevMessageId = -1;
        private NetworkWriter transformWriter;
        // ---

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
            transformWriter = new NetworkWriter ();
            InvokeRepeating ("TryInit", 0, 0.1f);
        }

        private void TryInit () {
            if (Init ()) {
                CancelInvoke ("TryInit");
            }
        }

        public void ResetTarget () {
            localTargetState = RigidbodyStateUtil.BuildRigidbodyState (transform, rb, SyncVelocity);
            serverTargetState = localTargetState.ToServer (mode == NetworkTransformMode.Rigidbody);
        }

        protected bool Init () {
            if (!TransformUtil.IsInitialized) return false;

            switch (mode) {
                case NetworkTransformMode.Agent:
                    agent.Warp (startPosition.ToLocal ());
                    transform.rotation = startRotation.ToLocal ();
                    agent.enabled = true;
                    break;
                case NetworkTransformMode.Rigidbody:
                    localTargetState = rb.ToLocalState ();
                    serverTargetState = rb.ToServerState ();
                    transform.MoveToLocal ();
                    break;
                default:
                    localTargetState = transform.ToLocalState ();
                    serverTargetState = transform.ToServerState ();
                    transform.MoveToLocal ();
                    break;
            }

            return isInitialized = true;
        }

        protected virtual void Update () {
            if (!IsSender || !sendUpdates || !isInitialized || !ARManager.WorldEnabled) return;

            wasSenderInLastFrame = true;

            if (PastThreshold ()) {
                RigidbodyStateUtil.SaveToServerState (ref savedRbs, transform, rb, SyncVelocity);
                RelativeNetworkTransformMessageHandler.SendTransformToServer (
                    transformWriter, savedRbs, nextMessageId++, netId, SyncVelocity);
                wait = 0;
            } else {
                wait += Time.deltaTime;
            }
        }

        protected virtual void FixedUpdate () {
            if (!sendUpdates || IsSender || !isInitialized) {
                return;
            }

            if (wasSenderInLastFrame) {
                wasSenderInLastFrame = false;
                ResetTarget ();
            } else {
#if UNITY_WSA && !UNITY_EDITOR
                localTargetState = serverTargetState.ToLocal (mode == NetworkTransformMode.Rigidbody);
#endif
            }

            LerpToLocalTarget ();
        }

        private void LerpToLocalTarget () {
            switch (mode) {
                case NetworkTransformMode.Rigidbody:
                    if (!rb) return;
                    rb.MovePosition (Vector3.Lerp (rb.position, localTargetState.position, movementInterpolation));
                    rb.MoveRotation (Quaternion.Lerp (rb.rotation, localTargetState.rotation, rotationInterpolation));
                    rb.velocity = Vector3.Lerp (rb.velocity, localTargetState.velocity, movementInterpolation);
                    rb.angularVelocity = Vector3.Lerp (rb.angularVelocity, localTargetState.angularVelocity, rotationInterpolation);
                    break;
                case NetworkTransformMode.Transform:
                    transform.position = Vector3.Lerp (transform.position, localTargetState.position, movementInterpolation);
                    transform.rotation = Quaternion.Lerp (transform.rotation, localTargetState.rotation, rotationInterpolation);
                    break;
            }
        }

        public void UpdatePosition (RigidbodyState serverRbs, int messageId) {
            if (IsSender || !isInitialized || messageId < prevMessageId) return;

            var localRbs = serverRbs.ToLocal (SyncVelocity);
            if (!localRbs.IsValid (SyncVelocity)) return;

            prevMessageId = messageId;
            wasSenderInLastFrame = false;

            localTargetState = localRbs;
            serverTargetState = serverRbs;

            if (mode == NetworkTransformMode.Agent) {
                UpdateAgentPosition (localTargetState);
            }
        }

        private void UpdateAgentPosition (RigidbodyState newLocalState) {
            if (agent.isOnNavMesh) {
                navMeshMissCount = 0;
                agent.Move (newLocalState.position - transform.position);
                agent.transform.rotation = newLocalState.rotation;
            } else {
                navMeshMissCount++;
                if (navMeshMissCount > 5) {
                    agent.Warp (newLocalState.position);
                }
            }
        }

        private bool PastThreshold () {
            if (wait > maxWait) {
                return true;
            }

            if (TransformUtil.IsCentered) {
                return Vector3.Distance (transform.position, savedRbs.position) > positionThreshold ||
                    Quaternion.Angle (transform.rotation, savedRbs.rotation) > rotationThreshold;
            } else {
                return Vector3.Distance (transform.position.ToServer (), savedRbs.position) > positionThreshold ||
                    Quaternion.Angle (transform.rotation.ToServer (), savedRbs.rotation) > rotationThreshold;
            }
        }

        /*[Command]
        private void CmdSyncPosition (RigidbodyState rigidbodyState, int messageId) {
            RpcBroadcastPosition (rigidbodyState, messageId);
        }

        [ClientRpc]
        private void RpcBroadcastPosition (RigidbodyState rbs, int messageId) {
            UpdatePosition (rbs, messageId);
        }*/
    }
}