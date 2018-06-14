#if (UNITY_WSA || UNITY_EDITOR)

using System;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Interaction.HMD.Gestures {
    public class ClickSelectNavigationRotateAndManipulationMoveGestures : ClickSelectAndNavigationRotateGestures, IManipulationHandler {

        protected GestureFunction? manipulationFunction;
        protected bool isManipulating;
        private bool resetAfterMove;

        private const float minScale = 2f;
        private const float maxScale = 6.0f;
        private Vector3 prevCumulativeDelta;
        private Vector3 prevScaledCumulativeDelta;
        private float maxDetectedMagnitude = 0.000001f;

        protected Vector3 scaledCumulativeDelta;

        private void Start () {
            Reset (disableMoving: true);
        }

        protected override void OnEnable() {
            base.OnEnable ();
            Reset (disableMoving: true);
        }

        protected virtual void Update () {
            if (resetAfterMove && (!StateManager || !StateManager.IsMoving ())) {
                Reset (disableMoving: true);
            }
        }

        protected void OnDisable () {
            Reset (disableMoving: false);
        }

        protected virtual void Reset (bool disableMoving) {
            isManipulating = false;
            resetAfterMove = false;
            if (StateManager) {
                StateManager.MovingDisabled = disableMoving;
            }
        }

        public override void OnManipulationStarted (ManipulationEventData eventData) {
            if (!manipulationFunction.HasValue) {
                throw new ArgumentNullException ("manipulationFunction",
                    "manipulationFunction must be set before manipulation events occur");
            }

            if ((StateManager.IsHovering () && IsDetectedGestureFunction (manipulationFunction.Value)) ||
                (StateManager.IsSpraying () && IsDetectedGestureFunction (GestureFunction.Select))) {
                isManipulating = true;
                StateManager.MovingDisabled = false;
                prevCumulativeDelta = Vector3.zero;
                prevScaledCumulativeDelta = Vector3.zero;
            }
            base.OnManipulationStarted (eventData);
        }

        public override void OnManipulationUpdated (ManipulationEventData eventData) {
            base.OnManipulationUpdated (eventData);
            if (isManipulating) {
                // Get the new movement
                Vector3 delta = eventData.CumulativeDelta - prevCumulativeDelta;

                // Update the max magnitude
                float deltaMagnitude = delta.magnitude;
                maxDetectedMagnitude = Mathf.Max (maxDetectedMagnitude, deltaMagnitude);

                // Scale the new movement
                float percent = deltaMagnitude / maxDetectedMagnitude;
                float scale = (maxScale - minScale) * percent + minScale;
                Vector3 scaledDelta = delta * TransformUtil.LocalRadius * scale;

                // Add to total
                scaledCumulativeDelta = prevScaledCumulativeDelta + scaledDelta;

                // Update previous values
                prevScaledCumulativeDelta = scaledCumulativeDelta;
                prevCumulativeDelta = eventData.CumulativeDelta;
            }
        }

        public override void OnManipulationCompleted (ManipulationEventData eventData) {
            if (isManipulating) {
                resetAfterMove = true;
            }
            base.OnManipulationCompleted (eventData);
        }

        public override void OnManipulationCanceled (ManipulationEventData eventData) {
            if (isManipulating) {
                resetAfterMove = true;
            }
            base.OnManipulationCanceled (eventData);
        }
    }
}

#endif