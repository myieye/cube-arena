using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Utils
{
	public class Colourer : NetworkBehaviour {

		[SyncVar]
		public Color color;
		protected Renderer rend;

		public virtual void Start () {
			rend = GetComponent<Renderer>();
			SetColor(color);
		}

		protected void SetColor(Color color) {
			rend.material.color = color;
		}
	}
}