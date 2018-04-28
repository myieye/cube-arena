using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.GameObjects.AR {

	public class ARObject : MonoBehaviour {
		public virtual void Start () {
			if (Settings.Instance.AREnabled) {
				ARManager.Instance.RegisterARObject (this);
			}
		}
	}
}