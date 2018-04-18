using UnityEngine;

namespace CubeArena.Assets.MyScripts.GameObjects.AR {
    public class DummyTrackableEventHandler : MonoBehaviour {
        protected virtual void Start () { }
        protected virtual void OnTrackingFound () { }
        protected virtual void OnTrackingLost () { }
    }
}