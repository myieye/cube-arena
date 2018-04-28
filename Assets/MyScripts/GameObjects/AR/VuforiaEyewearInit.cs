using UnityEngine;

#if !UNITY_STANDALONE
using Vuforia;
#endif

namespace CubeArena.Assets.MyScripts.GameObjects.AR {

    public class VuforiaEyewearInit : MonoBehaviour {

        void Awake () {
#if !UNITY_STANDALONE
            VuforiaARController.Instance.RegisterVuforiaInitializedCallback (OnVuforiaStarted);
#endif
        }

        void OnVuforiaStarted () {
            Debug.Log ("OnVuforiaStarted() called.");
#if !UNITY_EDITOR
#if UNITY_WSA
            DigitalEyewearARController.Instance.SetEyewearType (DigitalEyewearARController.EyewearType.OpticalSeeThrough);
            DigitalEyewearARController.Instance.SetSeeThroughConfiguration (DigitalEyewearARController.SeeThroughConfiguration.HoloLens);
#elif UNITY_ANDROID
            DigitalEyewearARController.Instance.SetEyewearType (DigitalEyewearARController.EyewearType.None);
#endif
#endif
        }
    }
}