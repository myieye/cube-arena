using UnityEngine;

namespace CubeArena.Assets.MyScripts.Interaction.Util {
	public static class TouchInput {
		public static bool HasSinglePOC () {
#if !UNITY_ANDROID
			return Input.GetMouseButton (0);
#else
			return Input.touchCount == 1;
#endif
		}

		public static bool POCIsFresh (int pocIndex) {
#if !UNITY_ANDROID
			return Input.GetMouseButtonDown (0);
#else
			return Input.touches[pocIndex].phase.Equals (TouchPhase.Began);
#endif
		}

		public static Vector2 GetPOCPosition (int pocIndex) {
#if !UNITY_ANDROID
			return Input.mousePosition;
#else
			return Input.touches[pocIndex].position;
#endif
		}

		public static bool NoPOCs () {
#if !UNITY_ANDROID
			return !Input.GetMouseButton (0);
#else
			return Input.touchCount == 0;
#endif
		}

		public static bool GetPOCDown (int pocIndex) {
#if !UNITY_ANDROID
			return GetPOC (pocIndex) && Input.GetMouseButtonDown (pocIndex);
#else
			return GetPOC (pocIndex) && Input.GetTouch (pocIndex).phase.Equals (TouchPhase.Began);
#endif
		}

		public static bool GetPOCUp (int pocIndex) {
#if !UNITY_ANDROID
			return GetPOC (pocIndex) && Input.GetMouseButtonUp (pocIndex);
#else
			return GetPOC (pocIndex) &&
				(Input.GetTouch (pocIndex).phase.Equals (TouchPhase.Ended) ||
					Input.GetTouch (pocIndex).phase.Equals (TouchPhase.Canceled));
#endif
		}

		public static bool GetPOC (int pocIndex) {
#if !UNITY_ANDROID
			return pocIndex == 0 && Input.GetMouseButton (0);
#else
			return pocIndex < Input.touchCount;
#endif
		}
	}
}