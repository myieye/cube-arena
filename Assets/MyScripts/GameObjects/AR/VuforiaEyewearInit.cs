using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;

#if !UNITY_STANDALONE
using Vuforia;
#endif

namespace CubeArena.Assets.MyScripts.GameObjects.AR {

    public class VuforiaEyewearInit : MonoBehaviour {

        void Awake () {
#if !UNITY_STANDALONE || UNITY_EDITOR
            VuforiaARController.Instance.RegisterVuforiaInitializedCallback (OnVuforiaInitialized);
#endif
        }

        void OnVuforiaInitialized () {
#if UNITY_WSA
            DigitalEyewearARController.Instance.SetEyewearType (DigitalEyewearARController.EyewearType.OpticalSeeThrough);
            DigitalEyewearARController.Instance.SetSeeThroughConfiguration (DigitalEyewearARController.SeeThroughConfiguration.HoloLens);
#elif UNITY_ANDROID
            DigitalEyewearARController.Instance.SetEyewearType (DigitalEyewearARController.EyewearType.None);
            VuforiaARController.Instance.SetWorldCenterMode (VuforiaARController.WorldCenterMode.FIRST_TARGET);
#endif
        }
    }
}