using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Utils {
	public class Colourer : NetworkBehaviour {

		[HideInInspector]
		[SyncVar (hook = "SetColour")]
		public Color color;
		protected Renderer rend;

		public virtual void Start () {
			rend = GetComponent<Renderer> ();
			SetColour (color);
		}

		protected void SetColour (Color color) {
			if (rend) {
				rend.material.color = color;
			}
		}
	}
}