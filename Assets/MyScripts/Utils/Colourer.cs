using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Utils
{
	public class Colourer : NetworkBehaviour {

		[SyncVar/*(hook="SetColor")*/]
		public Color color;
		protected Renderer[] renderers;

		public virtual void Start () {
			renderers = GetComponentsInChildren<Renderer>();
			SetColor(color);
		}

		protected void SetColor(Color color) {
			foreach (var rend in renderers)
				rend.material.color = color;
		}
	}
}