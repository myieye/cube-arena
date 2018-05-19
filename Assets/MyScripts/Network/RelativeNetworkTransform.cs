﻿using System;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.Utils;
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

        private NetworkTransformMode mode;
        private Rigidbody rb;
        private NavMeshAgent agent;
        private RigidbodyState rbs = new RigidbodyState ();
        private float wait = 0;
        protected bool isInitialized;
        public bool IsSender {
            get {
                return hasAuthority || (!localPlayerAuthority && isServer);
            }
        }
        private Vector3 startPosition;
        private Quaternion startRotation;

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
            if (!hasAuthority || TransformUtil.IsCentered || Init()) {
                CancelInvoke ("TryInit");
                if (mode == NetworkTransformMode.Agent) {
                    agent.enabled = true;
                }
            }
        }

        protected bool Init () {
            if (!TransformUtil.IsInitialized) return false;

            switch (mode) {
                case NetworkTransformMode.Agent:
                    //agent.enabled = false;
                    agent.Warp (startPosition.ToLocal ());
                    transform.rotation = startRotation.ToLocal ();
                    break;
                default:
                    transform.position = startPosition.ToLocal ();
                    transform.rotation = startRotation.ToLocal ();
                    break;
            }

            return isInitialized = true;
        }

        protected virtual void Update () {
            if (!IsSender) return;

            wait = Mathf.Lerp (wait, MaxWait, Time.deltaTime * interpolationSpeed);
            if (PastThreshold ()) {
                TransmitSync ();
                SaveState ();
            }
        }

        void TransmitSync () {
            var relativeRbs = CalcStateInServerCoordinates ();
            CmdSyncPosition (relativeRbs);
        }

        [Command]
        private void CmdSyncPosition (RigidbodyState rigidbodyState) {
            RpcBroadcastPosition (rigidbodyState);
        }

        [ClientRpc]
        private void RpcBroadcastPosition (RigidbodyState rigidbodyState) {
            if (IsSender/* || !isInitialized*/) return;

            rigidbodyState = TransformToLocalCoordinates (rigidbodyState);

            switch (mode) {
                case NetworkTransformMode.Rigidbody:
                    if (!rb) return;
                    rb.MovePosition (rigidbodyState.position);
                    rb.MoveRotation (rigidbodyState.rotation);
                    rb.velocity = rigidbodyState.velocity;
                    rb.angularVelocity = rigidbodyState.angularVelocity;
                    break;
                case NetworkTransformMode.Transform:
                    transform.position = rigidbodyState.position;
                    transform.rotation = rigidbodyState.rotation;
                    break;
                case NetworkTransformMode.Agent:
                    agent.Move (rigidbodyState.position - transform.position);
                    agent.transform.rotation = rigidbodyState.rotation;
                    break;
            }
        }

        private RigidbodyState CalcStateInServerCoordinates () {
            if (mode == NetworkTransformMode.Rigidbody) {
                return rb.ToServer ();
            } else {
                return transform.ToServer ();
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