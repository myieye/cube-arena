using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.AR
{
	public interface ARObject {
		void SetArActive(bool arEnabled);
		GameObject gameObject { get; }
	}
}