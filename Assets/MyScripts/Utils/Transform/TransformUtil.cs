using System;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Utils.TransformUtils {
    public class TransformUtil : NetworkBehaviour {

        [SerializeField]
        private Collider ground;
        public static Transform World { get; private set; }
        public static Collider Ground { get; private set; }

        [SyncVar (hook = "OnServerRadiusChange")]
        private float _serverRadius;
        private static float ServerRadius { get; set; }
        public static float LocalRadius { get; private set; }

        private static bool ShouldTransform {
            get {
                return World && !IsCentered;
            }
        }

        private static bool IsCentered {
            get {
                return World && World.position == Vector3.zero &&
                    World.rotation == Quaternion.identity;
            }
        }

        void Start () {
            if (!ground) {
                ground = GameObject.Find (Names.Ground).GetComponent<Collider> ();
            }
            Ground = ground;
            World = ground.transform;

#if UNITY_WSA && !UNITY_EDITOR
            InvokeRepeating ("UpdateLocalRadius", 0, 0.5f);
#else 
            LocalRadius = ground.bounds.center.x - ground.bounds.min.x;
#endif

            if (isServer) {
                _serverRadius = LocalRadius;
            }
        }

        public override void OnStartClient () {
            base.OnStartClient ();
            OnServerRadiusChange (_serverRadius);
        }

        private void UpdateLocalRadius () {
            LocalRadius = ground.bounds.center.x - ground.bounds.min.x;
        }

        private void OnServerRadiusChange (float newServerRadius) {
            ServerRadius = newServerRadius;
        }

        public static RigidbodyState Transform (TransformDirection direction, Transform transform) {
            var rbs = TransformToRigidbodyState (transform);
            return Transform (direction, ref rbs, false);
        }

        public static RigidbodyState Transform (TransformDirection direction, Rigidbody rigidbody) {
            var rbs = RigidbodyToRigidbodyState (rigidbody);
            return Transform (direction, ref rbs, true);
        }

        public static RigidbodyState Transform (TransformDirection direction, ref RigidbodyState fromRbs, bool transformVelocity) {
            if (!ShouldTransform) return fromRbs;

            var toRbs = new RigidbodyState {
                position = Transform (direction, fromRbs.position),
                rotation = Transform (direction, fromRbs.rotation)
            };

            if (transformVelocity) {
                CalcNewVelocity (direction, ref fromRbs, ref toRbs);
            }
            return toRbs;
        }

        public static Vector3 Transform (TransformDirection direction, Vector3 pos) {
            if (!ShouldTransform) return pos;

            switch (direction) {
                case TransformDirection.LocalToServer:
                    return TransformToServerCoordinates (pos);
                case TransformDirection.ServerToLocal:
                    return TransformToLocalCoordinates (pos);
                default:
                    throw new InvalidTransformDirectionException (direction);
            }
        }

        public static Quaternion Transform (TransformDirection direction, Quaternion rot) {
            if (!ShouldTransform) return rot;

            switch (direction) {
                case TransformDirection.LocalToServer:
                    return TransformToServerCoordinates (rot);
                case TransformDirection.ServerToLocal:
                    return TransformToLocalCoordinates (rot);
                default:
                    throw new InvalidTransformDirectionException (direction);
            }
        }

        public static void MoveToServerCoordinates (Transform transform) {
            if (!ShouldTransform) return;

            transform.position = TransformToServerCoordinates (transform.position);
            transform.rotation = TransformToServerCoordinates (transform.rotation);
        }

        public static void MoveToLocalCoordinates (Transform transform) {
            if (!ShouldTransform) return;

            transform.position = TransformToLocalCoordinates (transform.position);
            transform.rotation = TransformToLocalCoordinates (transform.rotation);
        }

        private static Vector3 TransformToServerCoordinates (Vector3 pos) {
            pos = pos - World.transform.position;
            //pos = TransformToServerScale (pos);
#if UNITY_WSA && !UNITY_EDITOR
            pos = Quaternion.Euler (0, 180, 0) * pos;
#endif
            return pos;
        }

        private static Vector3 TransformToLocalCoordinates (Vector3 pos) {
            //pos = TransformToLocalScale (pos);
#if UNITY_WSA && !UNITY_EDITOR
            pos = Quaternion.Euler (0, 180, 0) * pos;
#endif
            return pos + World.transform.position;
        }

        private static Quaternion TransformToServerCoordinates (Quaternion rot) {
            rot = rot * Quaternion.Inverse (World.transform.rotation);
#if UNITY_WSA && !UNITY_EDITOR
            rot = Quaternion.Inverse (rot);
#endif
            return rot;
        }

        private static Quaternion TransformToLocalCoordinates (Quaternion rot) {
#if UNITY_WSA && !UNITY_EDITOR
            rot = Quaternion.Inverse (rot);
#endif
            return rot * World.transform.rotation;
        }

        private static void CalcNewVelocity (TransformDirection direction, ref RigidbodyState from, ref RigidbodyState to) {
            Quaternion rotation;
            switch (direction) {
                case TransformDirection.LocalToServer:
                    rotation = from.rotation * Quaternion.Inverse (to.rotation);
                    break;
                case TransformDirection.ServerToLocal:
                    rotation = World.rotation * Quaternion.Inverse (to.rotation);
                    break;
                default:
                    throw new InvalidTransformDirectionException (direction);
            }
            to.velocity = rotation * from.velocity;
            to.angularVelocity = rotation * from.angularVelocity;
        }

        private static Vector3 TransformToServerScale (Vector3 pos) {
            var y = pos.y;
            pos = (pos / LocalRadius) * ServerRadius;
            pos.y = y;
            return pos;
        }

        private static Vector3 TransformToLocalScale (Vector3 pos) {
            var y = pos.y;
            pos = pos * (LocalRadius / ServerRadius);
            pos.y = y;
            return pos;
        }

        private static RigidbodyState TransformToRigidbodyState (Transform transform) {
            return new RigidbodyState {
                position = transform.position,
                    rotation = transform.rotation
            };
        }

        private static RigidbodyState RigidbodyToRigidbodyState (Rigidbody rigidbody) {
            return new RigidbodyState {
                position = rigidbody.position,
                    rotation = rigidbody.rotation,
                    velocity = rigidbody.velocity,
                    angularVelocity = rigidbody.angularVelocity
            };
        }

        public static void ClampInArea (Transform transform, float extent) {
            var currServerPoint = TransformToServerCoordinates (transform.position);

            var max = LocalRadius - extent;
            var outOfBounds = false;

            if (currServerPoint.x < -max || currServerPoint.x > max) {
                outOfBounds = true;
                currServerPoint.x = Mathf.Clamp (currServerPoint.x, -max, max);
            }
            if (currServerPoint.z < -max || currServerPoint.z > max) {
                outOfBounds = true;
                currServerPoint.z = Mathf.Clamp (currServerPoint.z, -max, max);
            }
            if (currServerPoint.y < 0 || currServerPoint.y > max) {
                outOfBounds = true;
                currServerPoint.y = Mathf.Clamp (currServerPoint.y, extent, max);
            }

            if (outOfBounds) {
                transform.position = TransformToLocalCoordinates (currServerPoint);
            }
        }

        public static Vector3 GetRandomPosition () {
            return new Vector3 (
                UnityEngine.Random.Range (-LocalRadius, LocalRadius),
                0,
                UnityEngine.Random.Range (-LocalRadius, LocalRadius));
        }

        public static Vector3 GetRandomLocalPosition () {
            return Transform (TransformDirection.ServerToLocal, GetRandomPosition ());
        }

        public static Vector3 GetRandomNavMeshPosition () {
            return ToNavMeshPosition (GetRandomLocalPosition ());
        }

        public static Vector3 ToNavMeshPosition (Vector3 position) {
            NavMeshHit hit;
            NavMesh.SamplePosition (position, out hit, 1.0f, NavMesh.AllAreas);
            return hit.position;
        }
    }
}