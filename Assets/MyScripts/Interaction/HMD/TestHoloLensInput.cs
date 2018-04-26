using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Interaction.HMD {
    public class TestHoloLensInput : MonoBehaviour, IInputHandler, IInputClickHandler, ISourceRotationHandler, INavigationHandler {

        private InputManager instance;
        private List<InputSourceInfo> sources;

        void Start () {
            instance = InputManager.Instance;
            sources = new List<InputSourceInfo> ();
        }

        void Update () {
            if (instance.DetectedInputSources.Count > 0) {
                foreach (var source in instance.DetectedInputSources) {
                    if (!sources.Contains (source)) {
                        sources.Add (source);
                    }
                }
            }
        }

        void IInputHandler.OnInputDown (InputEventData eventData) {
            Debug.Log ("IInputHandler.OnInputDown");
        }

        void IInputHandler.OnInputUp (InputEventData eventData) {
            Debug.Log ("IInputHandler.OnInputUp");
        }

        void IInputClickHandler.OnInputClicked (InputClickedEventData eventData) {
            Debug.Log ("IInputClickHandler.OnInputClicked");
        }

        void ISourceRotationHandler.OnRotationChanged (SourceRotationEventData eventData) {
            Debug.Log ("ISourceRotationHandler.OnRotationChanged");
        }

        void INavigationHandler.OnNavigationStarted (NavigationEventData eventData) {
            Debug.Log ("OnNavigationStarted");
        }

        void INavigationHandler.OnNavigationUpdated (NavigationEventData eventData) {
            Debug.Log ("OnNavigationStarted: " + eventData.NormalizedOffset);
        }

        void INavigationHandler.OnNavigationCompleted (NavigationEventData eventData) {
            Debug.Log ("OnNavigationStarted");
        }

        void INavigationHandler.OnNavigationCanceled (NavigationEventData eventData) {
            Debug.Log ("OnNavigationStarted");
        }
    }
}