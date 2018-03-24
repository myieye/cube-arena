using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Fire
{
	public interface FireSource {
		bool HasSource();
		GameObject gameObject { get; }
	}
}