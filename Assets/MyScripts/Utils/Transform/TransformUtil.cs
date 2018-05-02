using System;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Utils.TransformUtils {
    public class TransformUtil : NetworkBehaviour {

        [SerializeField]
        private Transform origin;
        public static Transform Origin { get; private set; }

        [SyncVar (hook = "OnServerRadiusChange")]
        private float _serverRadius;
        private static float ServerRadius { get; set; }
        public static float LocalRadius { get; private set; }
        private Collider coll;

        void Start () {
            coll = GameObject.Find (Names.Ground).GetComponent<Collider> ();

            if (!origin) {
                origin = coll.transform;
            }

            Origin = origin;

#if UNITY_WSA && !UNITY_EDITOR
            InvokeRepeating ("UpdateLocalRadius", 0, 0.5f);
#else 
            LocalRadius = coll.bounds.center.x - coll.bounds.min.x;
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
            LocalRadius = coll.bounds.center.x - coll.bounds.min.x;
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
            if (!Origin || IsCentered (Origin)) return fromRbs;

            var toRbs = new RigidbodyState {
                position = Transform (direction, fromRbs.position),
                rotation = Transform (direction, fromRbs.rotation)
            };

            if (transformVelocity) {
                CalcNewVelocity (direction, fromRbs, ref toRbs);
            }
            return toRbs;
        }

        public static Vector3 Transform (TransformDirection direction, Vector3 pos) {
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
            switch (direction) {
                case TransformDirection.LocalToServer:
                    return TransformToServerCoordinates (rot);
                case TransformDirection.ServerToLocal:
                    return TransformToLocalCoordinates (rot);
                default:
                    throw new InvalidTransformDirectionException (direction);
            }
        }

        public static void MoveToLocalCoordinates (Transform transform) {
            if (!Origin || IsCentered (Origin)) return;

            transform.position = TransformToLocalCoordinates (transform.position);
            transform.rotation = TransformToLocalCoordinates (transform.rotation);
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

        private static Vector3 TransformToServerScale (Vector3 pos) {
            var y = pos.y;
            pos = (pos / LocalRadius) * ServerRadius;
            //pos.y = y;
            return pos;
        }

        private static Vector3 TransformToLocalScale (Vector3 pos) {
            var y = pos.y;
            pos = pos * (LocalRadius / ServerRadius);
            //pos.y = y;
            return pos;
        }

        private static void CalcNewVelocity (TransformDirection direction,
            RigidbodyState from, ref RigidbodyState to) {
            Quaternion rotation;
            switch (direction) {
                case TransformDirection.LocalToServer:
                    rotation = from.rotation * Quaternion.Inverse (to.rotation);
                    break;
                case TransformDirection.ServerToLocal:
                    rotation = Origin.rotation * Quaternion.Inverse (to.rotation);
                    break;
                default:
                    throw new InvalidTransformDirectionException (direction);
            }
            to.velocity = rotation * from.velocity;
            to.angularVelocity = rotation * from.angularVelocity;
        }

        private static Vector3 TransformToServerCoordinates (Vector3 pos) {
            pos = pos - Origin.transform.position;
            pos = TransformToServerScale (pos);
#if UNITY_WSA && !UNITY_EDITOR
            pos = Quaternion.Euler (0, 180, 0) * pos;
#endif
            return pos;
        }

        private static Quaternion TransformToServerCoordinates (Quaternion rot) {
            rot = rot * Quaternion.Inverse (Origin.transform.rotation);
#if UNITY_WSA && !UNITY_EDITOR
            rot = Quaternion.Inverse (rot);
#endif
            return rot;
        }

        private static Vector3 TransformToLocalCoordinates (Vector3 pos) {
            pos = TransformToLocalScale (pos);
#if UNITY_WSA && !UNITY_EDITOR
            pos = Quaternion.Euler (0, 180, 0) * pos;
#endif
            return pos + Origin.transform.position;
        }

        private static Quaternion TransformToLocalCoordinates (Quaternion rot) {
#if UNITY_WSA && !UNITY_EDITOR
            rot = Quaternion.Inverse (rot);
#endif
            return rot * Origin.transform.rotation;
        }

        private static bool IsCentered (Transform transform) {
            return transform.position == Vector3.zero &&
                transform.rotation == Quaternion.identity;
        }
    }
}