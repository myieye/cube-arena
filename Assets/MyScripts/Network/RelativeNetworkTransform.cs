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
        private float positionThreshold = 0.01f;
        [SerializeField]
        [ConditionalField ("sendUpdates", true)]
        private float rotationThreshold = 1f;
        /*
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
         */
        private int navMeshMissCount;
        private NetworkTransformMode mode;
        private Rigidbody rb;
        private NavMeshAgent agent;
        private RigidbodyState rbs = new RigidbodyState ();
        private RigidbodyState localTargetState = new RigidbodyState ();
        private RigidbodyState serverTargetState = new RigidbodyState ();

        private bool wasSenderInLastFrame;

        //private float wait = 0;
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

        public void ResetTarget () {
            localTargetState = GetCurrentState ();
            serverTargetState = CalcStateInServerCoordinates ();
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
            if (!IsSender || !sendUpdates || !isInitialized) return;

            wasSenderInLastFrame = true;

            //wait = Mathf.Lerp (wait, MaxWait, Time.deltaTime * interpolationSpeed);
            if (PastThreshold ()) {
                TransmitSync ();
                SaveState ();
            }
        }

        /**
        Testing fix for problem: HoloLens constantly Lerps to targetState, which is a fixed state,
            Not a local state. It actually needs to save the target and reconvert on every fixed update.
         */
        protected virtual void FixedUpdate () {
            if (!sendUpdates || IsSender || !isInitialized) {
                return;
            }

            if (wasSenderInLastFrame) {
                wasSenderInLastFrame = false;
                
            }

#if UNITY_WSA && !UNITY_EDITOR
            localTargetState = serverTargetState.ToLocal (mode == NetworkTransformMode.Rigidbody);
#endif

            // TODO: Try not lerping
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

        void TransmitSync () {
            var relativeRbs = CalcStateInServerCoordinates ();
            CmdSyncPosition (relativeRbs, nextMessageId++);
        }

        // TODO: Don't call and see if Command without authority errors show up.
        [Command]
        private void CmdSyncPosition (RigidbodyState rigidbodyState, long messageId) {
            RpcBroadcastPosition (rigidbodyState, messageId);
        }

        [ClientRpc]
        private void RpcBroadcastPosition (RigidbodyState rbs, long messageId) {
            if (IsSender || !isInitialized || messageId < prevMessageId) return;

            prevMessageId = messageId;
            var localRbs = TransformToLocalCoordinates (rbs);
            if (!IsValid (localRbs)) return;

            localTargetState = localRbs;
            serverTargetState = rbs;

            switch (mode) {
                /*case NetworkTransformMode.Rigidbody:
                    if (!rb) return;
                    rb.MovePosition (localRigidbodyState.position);
                    rb.MoveRotation (localRigidbodyState.rotation);
                    rb.velocity = localRigidbodyState.velocity;
                    rb.angularVelocity = localRigidbodyState.angularVelocity;
                    break;
                case NetworkTransformMode.Transform:
                    transform.position = localRigidbodyState.position;
                    transform.rotation = localRigidbodyState.rotation;
                    break;*/
                case NetworkTransformMode.Agent:
                    if (agent.isOnNavMesh) {
                        navMeshMissCount = 0;
                        agent.Move (localRbs.position - transform.position);
                        agent.transform.rotation = localRbs.rotation;
                    } else {
                        navMeshMissCount++;
                        if (navMeshMissCount > 5) {
                            agent.Warp (localRbs.position);
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
            return
            Vector3.Distance (transform.position, rbs.position) > positionThreshold ||
                Quaternion.Angle (transform.rotation, rbs.rotation) > rotationThreshold;
        }

        private RigidbodyState GetCurrentState () {
            var rbs = new RigidbodyState ();
            if (mode == NetworkTransformMode.Rigidbody && rb) {
                rbs.position = rb.position;
                rbs.rotation = rb.rotation;
                rbs.velocity = rb.velocity;
                rbs.angularVelocity = rb.angularVelocity;
            } else {
                rbs.position = transform.position;
                rbs.rotation = transform.rotation;
            }
            return rbs;
        }

        private void SaveState () {
            //wait = 0;
            if (mode == NetworkTransformMode.Rigidbody && rb) {
                rbs.position = rb.position;
                rbs.rotation = rb.rotation;
                rbs.velocity = rb.velocity;
                rbs.angularVelocity = rb.angularVelocity;
            } else {
                rbs.position = transform.position;
                rbs.rotation = transform.rotation;
            }
        }

        /*
        private float ScaleThreshold (float threshold) {
            return threshold * (1 - (wait / MaxWait));
        } */
    }
}