using System;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Utils.TransformUtils {
    public class TransformUtil : NetworkBehaviour {

        public static Transform World { get; private set; }
        public static BoxCollider Ground { get; private set; }

        [SyncVar (hook = "OnServerRadiusChange")]
        private float _serverRadius;
        public static float ServerRadius { get; private set; }
        public static float LocalRadius { get; private set; }

        public static bool IsInitialized { get; private set; }
        private static bool ShouldTransform {
            get {
                return IsInitialized && !IsCentered;
            }
        }
        public static bool IsCentered {
            get {
                return World && World.position == Vector3.zero &&
                    World.rotation == Quaternion.identity;
            }
        }
        private static bool matricesInitialized = false;
        //private static long matricesUpdateFrame = -1;
        private static Matrix4x4 localToWorldMatrix;
        private static Matrix4x4 worldToLocalMatrix;

        //private static long fixedFrameCount = 0;

        void Start () {
            Ground = GameObject.Find (Names.Ground).GetComponent<BoxCollider> ();
            World = Ground.transform;

            UpdateLocalRadius ();
#if UNITY_WSA && !UNITY_EDITOR
            InvokeRepeating ("UpdateLocalRadius", 0, 0.5f);
#endif

            if (isServer) {
                _serverRadius = LocalRadius;
            } else {
                OnServerRadiusChange (_serverRadius);
            }

            IsInitialized = true;
        }

        /*private void FixedUpdate () {
            fixedFrameCount++;
        }*/

        private static void CheckTransformMatricesReady () {
            // Only the HoloLens needs to repeatedly compute the matrices.
#if !UNITY_WSA || UNITY_EDITOR
            if (matricesInitialized) return;
#endif
            // TODO: Optimize without breaking.
            //if (matricesUpdateFrame == fixedFrameCount) return;

            localToWorldMatrix = Matrix4x4.TRS (World.position, World.rotation,
                Vector3.one * (LocalRadius / ServerRadius));
            worldToLocalMatrix = localToWorldMatrix.inverse;
            matricesInitialized = true;
            //matricesUpdateFrame = fixedFrameCount;
        }

        public override void OnStartClient () {
            base.OnStartClient ();
        }

        private void UpdateLocalRadius () {
            LocalRadius = (Ground.size.x * Ground.transform.lossyScale.x) / 2;
        }

        private void OnServerRadiusChange (float newServerRadius) {
            ServerRadius = newServerRadius;
            CheckTransformMatricesReady ();
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
                toRbs.velocity = TransformVector (direction, fromRbs.velocity);
                toRbs.angularVelocity = TransformVector (direction, fromRbs.angularVelocity);
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

        public static void ClampInArea (Transform transform, float radius) {
            if (!IsInitialized) return;

            var currServerPoint = TransformToServerCoordinates (transform.position);

            var max = LocalRadius - radius;
            var outOfBounds = false;

            if (currServerPoint.x < -max || currServerPoint.x > max) {
                outOfBounds = true;
                currServerPoint.x = Mathf.Clamp (currServerPoint.x, -max, max);
            }
            if (currServerPoint.z < -max || currServerPoint.z > max) {
                outOfBounds = true;
                currServerPoint.z = Mathf.Clamp (currServerPoint.z, -max, max);
            }
            if (currServerPoint.y < radius || currServerPoint.y > max) {
                outOfBounds = true;
                currServerPoint.y = Mathf.Clamp (currServerPoint.y, radius, max);
            }

            if (outOfBounds) {
                transform.position = TransformToLocalCoordinates (currServerPoint);
                var rb = transform.gameObject.GetComponent<Rigidbody> ();
                if (rb) {
                    rb.velocity = Vector3.zero;
                }
            }
        }

        public static Vector3 GetRandomPosition () {
            if (!IsInitialized) return Vector3.zero;

            return new Vector3 (
                UnityEngine.Random.Range (-LocalRadius, LocalRadius),
                0,
                UnityEngine.Random.Range (-LocalRadius, LocalRadius));
        }

        public static Vector3 GetRandomLocalPosition () {
            if (!IsInitialized) return Vector3.zero;

            return Transform (TransformDirection.ServerToLocal, GetRandomPosition ());
        }

        public static Vector3 GetRandomNavMeshPosition () {
            if (!IsInitialized) return Vector3.zero;

            return ToNavMeshPosition (GetRandomLocalPosition ()).Value;
        }

        public static Vector3? ToNavMeshPosition (Vector3 position) {
            NavMeshHit hit;
            if (!NavMesh.SamplePosition (position, out hit, 5.0f, NavMesh.AllAreas)) {
                Debug.LogWarning ("Failed to sample position");
            }
            return hit.position;
        }

        private static Vector3 TransformToServerCoordinates (Vector3 pos) {
            CheckTransformMatricesReady ();
            return worldToLocalMatrix.MultiplyPoint3x4 (pos);
        }

        private static Vector3 TransformToLocalCoordinates (Vector3 pos) {
            CheckTransformMatricesReady ();
            return localToWorldMatrix.MultiplyPoint3x4 (pos);
        }

        private static Quaternion TransformToServerCoordinates (Quaternion rot) {
            rot = Quaternion.Inverse (World.transform.rotation) * rot;
            return rot;
        }

        private static Quaternion TransformToLocalCoordinates (Quaternion rot) {
            return World.rotation * rot;
        }

        public static Vector3 TransformVector (TransformDirection direction, Vector3 vector) {
            if (!ShouldTransform) return vector;

            CheckTransformMatricesReady ();
            switch (direction) {
                case TransformDirection.LocalToServer:
                    return worldToLocalMatrix.MultiplyVector (vector);
                case TransformDirection.ServerToLocal:
                    return localToWorldMatrix.MultiplyVector (vector);
                default:
                    throw new InvalidTransformDirectionException (direction);
            }
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

        public static bool IsValid (Vector3 pos) {
            return !(
                float.IsNaN (pos.x) || float.IsInfinity (pos.x) ||
                float.IsNaN (pos.y) || float.IsInfinity (pos.y) ||
                float.IsNaN (pos.z) || float.IsInfinity (pos.z));
        }

        public static bool IsValid (Quaternion rotation) {
            return !(
                float.IsNaN (rotation.w) || float.IsInfinity (rotation.w) ||
                float.IsNaN (rotation.x) || float.IsInfinity (rotation.x) ||
                float.IsNaN (rotation.y) || float.IsInfinity (rotation.y) ||
                float.IsNaN (rotation.z) || float.IsInfinity (rotation.z));
        }
    }
}