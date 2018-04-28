using System.Linq;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
	public class ForDeviceTypes : MonoBehaviour {

		[SerializeField]
		private DeviceTypeSpec[] deviceTypes;

		void Awake () {
			if (!deviceTypes.Contains(DeviceTypeManager.DeviceType)) {
				Destroy (gameObject);
			}
		}
	}
}