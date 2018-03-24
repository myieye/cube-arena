using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/*
using Vuforia;

namespace CubeArena.Assets.MyScripts.Utils
{
    public class HideWorld : MonoBehaviour {

        // Use this for initialization
        void Start () {
            
        }

        bool show = true;
        
        void Update () {
            // Get the Vuforia StateManager
            StateManager sm = TrackerManager.Instance.GetStateManager ();
            
            bool hasTracker = sm.GetActiveTrackableBehaviours().Any();//.GetEnumerator().Current != null;
            //gameObject.SetActive(visible);

            if (hasTracker != show) {
                show = hasTracker;

                Debug.Log("Active: " + show);

                var rendererComponents = gameObject.GetComponentsInChildren<Renderer>(true);
                var colliderComponents = gameObject.GetComponentsInChildren<Collider>(true);
                var canvasComponents = gameObject.GetComponentsInChildren<Canvas>(true);

                // Enable rendering:
                foreach (var component in rendererComponents)
                    component.enabled = show;

                // Enable colliders:
                foreach (var component in colliderComponents)
                    component.enabled = show;

                // Enable canvas':
                foreach (var component in canvasComponents)
                    component.enabled = show;
            }

            // Query the StateManager to retrieve the list of
            // currently 'active' trackables 
            //(i.e. the ones currently being tracked by Vuforia)
            /*IEnumerable<TrackableBehaviour> activeTrackables = sm.GetActiveTrackableBehaviours ();
    
        // Iterate through the list of active trackables
            Debug.Log ("List of trackables currently active (tracked): ");
            foreach (TrackableBehaviour tb in activeTrackables) {
                tb.CurrentStatus
                Debug.Log("Trackable: " + tb.TrackableName);
            }
        //}
    //}
}
    */