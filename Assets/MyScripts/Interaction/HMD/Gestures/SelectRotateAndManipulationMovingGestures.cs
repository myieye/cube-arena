#if (UNITY_WSA || UNITY_EDITOR)

using System;
using HoloToolkit.Unity.InputModule;

namespace CubeArena.Assets.MyScripts.Interaction.HMD.Gestures {
    public class ClickSelectNavigationRotateAndManipulationMoveGestures : ClickSelectAndNavigationRotateGestures, IManipulationHandler {

        protected GestureFunction? manipulationFunction;
        protected bool isManipulating;
        private bool resetAfterMove;

        private void Start () {
            Reset (disableMoving: true);
        }

        protected override void OnEnable() {
            base.OnEnable ();
            Reset (disableMoving: true);
        }

        private void Update () {
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

            if (StateManager.IsHovering () && IsDetectedGestureFunction (manipulationFunction.Value)) {
                isManipulating = true;
                StateManager.MovingDisabled = false;
            }
            base.OnManipulationStarted (eventData);
        }

        public override void OnManipulationUpdated (ManipulationEventData eventData) {
            base.OnManipulationUpdated (eventData);
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