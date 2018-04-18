using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.GameObjects.AR {
	public interface ARObject {
		void SetArActive (bool arEnabled);
		GameObject gameObject { get; }
	}
}