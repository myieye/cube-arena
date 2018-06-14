using System.Diagnostics;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using NUnit.Framework;
using UnityEngine;

namespace CubeArena.Assets.Editor.Tests.Performance {
    public class TransformUtilPerformanceTest {

        private Matrix4x4 localToWorldMatrix;
        private Matrix4x4 worldToLocalMatrix;
        private Transform World;
        private RigidbodyState rbs;

        [SetUp]
        public void SetUp () {
            World = new GameObject ().transform;
            
            rbs = new RigidbodyState {
                position = new Vector3 (10, -15, 25),
                rotation = new Quaternion (0.7f, 1, 0.3f, 0.2f),
                velocity = new Vector3 (4, 9, -10),
                angularVelocity = new Vector3 (9, 2, 4)
            };
        }

        [TestCase (100, 1000)]
        public void TestMatrixGenerationPerformance (int warmUpCount, int testCount) {

            var stopwatch = Stopwatch.StartNew ();

            for (int i = 0; i < warmUpCount; i++) {
                GenerateMatrices ();
            }

            double warmUpTime = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset ();

            UnityEngine.Debug.LogFormat ("[Warm-Up] Generated: {0} matrices in {1}s. ({2}/s)",
                warmUpCount, warmUpTime, (warmUpCount / warmUpTime));

            for (int i = 0; i < testCount; i++) {
                GenerateMatrices ();
            }

            stopwatch.Stop ();
            double testTime = stopwatch.Elapsed.TotalSeconds;

            UnityEngine.Debug.LogFormat ("[Test] Generated: {0} matrices in {1}s. ({2}/s)",
                testCount, testTime, (testCount / testTime));
        }

        [TestCase (100, 1000)]
        public void TestRbsTransformPerformance (int warmUpCount, int testCount) {
            
            var stopwatch = Stopwatch.StartNew ();

            for (int i = 0; i < warmUpCount; i++) {
                rbs = Transform (TransformDirection.LocalToServer, ref rbs, true);
            }

            double warmUpTime = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset ();

            UnityEngine.Debug.LogFormat ("[Warm-Up] Performed: {0} transformations in {1}s. ({2}/s)",
                warmUpCount, warmUpTime, (warmUpCount / warmUpTime));

            for (int i = 0; i < testCount; i++) {
                rbs = Transform (TransformDirection.LocalToServer, ref rbs, true);
            }

            stopwatch.Stop ();
            double testTime = stopwatch.Elapsed.TotalSeconds;

            UnityEngine.Debug.LogFormat ("[Test] Performed: {0} transformations in {1}s. ({2}/s)",
                testCount, testTime, (testCount / testTime));
        }

        private void GenerateMatrices () {
            localToWorldMatrix = Matrix4x4.TRS (World.position, World.rotation,
                Vector3.one * (25.0f / 24.5f));
            worldToLocalMatrix = localToWorldMatrix.inverse;
        }

        private RigidbodyState Transform (TransformDirection direction, ref RigidbodyState fromRbs, bool transformVelocity) {
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
        
        private Vector3 Transform (TransformDirection direction, Vector3 pos) {
            switch (direction) {
                case TransformDirection.LocalToServer:
                    return TransformToServerCoordinates (pos);
                case TransformDirection.ServerToLocal:
                    return TransformToLocalCoordinates (pos);
                default:
                    throw new InvalidTransformDirectionException (direction);
            }
        }

        private Quaternion Transform (TransformDirection direction, Quaternion rot) {
            switch (direction) {
                case TransformDirection.LocalToServer:
                    return TransformToServerCoordinates (rot);
                case TransformDirection.ServerToLocal:
                    return TransformToLocalCoordinates (rot);
                default:
                    throw new InvalidTransformDirectionException (direction);
            }
        }
        
        private Vector3 TransformToServerCoordinates (Vector3 pos) {
            GenerateMatrices ();
            return worldToLocalMatrix.MultiplyPoint3x4 (pos);
        }

        private Vector3 TransformToLocalCoordinates (Vector3 pos) {
            GenerateMatrices ();
            return localToWorldMatrix.MultiplyPoint3x4 (pos);
        }

        private Quaternion TransformToServerCoordinates (Quaternion rot) {
            rot = Quaternion.Inverse (World.transform.rotation) * rot;
            return rot;
        }

        private Quaternion TransformToLocalCoordinates (Quaternion rot) {
            return World.rotation * rot;
        }

        private Vector3 TransformVector (TransformDirection direction, Vector3 vector) {
            GenerateMatrices ();
            switch (direction) {
                case TransformDirection.LocalToServer:
                    return worldToLocalMatrix.MultiplyVector (vector);
                case TransformDirection.ServerToLocal:
                    return localToWorldMatrix.MultiplyVector (vector);
                default:
                    throw new InvalidTransformDirectionException (direction);
            }
        }
    }
}