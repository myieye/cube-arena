using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.GameObjects.Fire {
	public class FireCube : MonoBehaviour {

		public FireSource Source { get; private set; }
		public List<FireSource> FireSources; // { get; private set; }
		private List<GameObject> fireObjects = new List<GameObject> ();
		public GameObject Cube {
			get {
				return transform.parent.gameObject;
			}
		}

		void Awake () {
			FireSources = new List<FireSource> ();
		}

		protected void OnTriggerEnter (Collider col) {
			if (IsFireObject (col.gameObject)) {
				fireObjects.Add (col.gameObject);
				FireSources.Add (col.GetComponentInParent<FireSource> ());
			}
		}

		protected void OnTriggerExit (Collider col) {
			if (IsFireObject (col.gameObject)) {
				var i = fireObjects.IndexOf (col.gameObject);
				var conn = FireSources[i];
				if (conn == Source) {
					Source = null;
				}
				FireSources.Remove (conn);
				fireObjects.Remove (col.gameObject);
			}
		}

		public bool CheckBurning () {
			Source = FireSources.Find (fireSource => fireSource.HasSource ());
			return Source != null;
		}

		private bool IsFireObject (GameObject obj) {
			return obj.CompareTag ("Fire") || obj.CompareTag ("Ground");
		}
	}
}