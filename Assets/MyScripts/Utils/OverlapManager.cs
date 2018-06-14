using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Utils {
	public class OverlapManager : MonoBehaviour {

		[SerializeField]
		private List<string> tagList = new List<string> ();
		[SerializeField]
		private List<string> ignoreList = new List<string> { "UI" };
		[SerializeField]
		public List<GameObject> touchedObjects = new List<GameObject> ();
		private Collider[] colliders;
		
		public virtual void Awake () {
			colliders = GetComponentsInChildren<Collider> (true);
		}

		public virtual void OnTriggerEnter (Collider col) {
			if (!OwnCollider(col) && HasMatchingTag (col.gameObject) && !touchedObjects.Contains (col.gameObject)) {
				touchedObjects.Add (col.gameObject);
			}
		}

		public virtual void OnTriggerExit (Collider col) {
			if (!OwnCollider(col) && HasMatchingTag (col.gameObject)) {
				touchedObjects.Remove (col.gameObject);
			}
		}

		public bool HasMatchingTag (GameObject obj) {
			CleanNulls ();
			if (ignoreList.Contains (obj.tag))
				return false;
			return !tagList.Any () || tagList.Contains (obj.tag);
		}

		public GameObject GetClosest () {
			CleanNulls ();
			GameObject closest;
			GetClosest (out closest);
			return closest;
		}

		public GameObject GetLocalClosest () {
			CleanNulls ();
			GameObject closest;
			GetLocalClosest (out closest);
			return closest;
		}

		public bool GetClosest (out GameObject closest) {
			CleanNulls ();
			return GetClosest (out closest, false);
		}

		public bool GetLocalClosest (out GameObject closest) {
			CleanNulls ();
			return GetClosest (out closest, true);
		}

		public int GetCount (string tag = null) {
			CleanNulls ();
			if (tag == null) {
				return touchedObjects.Count;
			} else {
				return touchedObjects.Count (obj => obj != null && obj.CompareTag (tag));
			}
		}

		private bool GetClosest (out GameObject closest, bool localOnly) {
			if (!HasOverlap (localOnly)) {
				closest = null;
				return false;
			}

			var minDist = float.MaxValue;
			closest = localOnly ? touchedObjects.First (IsLocal) : touchedObjects.First ();
			foreach (var gameObj in touchedObjects) {
				if (!localOnly || IsLocal (gameObj)) {
					var dist = Vector3.Distance (gameObj.transform.position, transform.position);
					if (dist < minDist) {
						minDist = dist;
						closest = gameObj;
					}
				}
			}
			return true;
		}

		public bool HasOverlapWith (GameObject obj) {
			return obj != null && touchedObjects.Contains (obj);
		}

		public bool HasOverlap () {
			CleanNulls ();
			return touchedObjects.Any ();
		}

		public bool HasLocalOverlap () {
			CleanNulls ();
			return touchedObjects.Any (IsLocal);
		}

		private bool HasOverlap (bool localOnly) {
			return localOnly ? HasLocalOverlap () : HasOverlap ();
		}

		private bool IsBelow (GameObject obj) {
			return obj.transform.position.y < this.transform.position.y;
		}

		private bool IsLocal (GameObject obj) {
			var netId = obj.GetComponent<NetworkIdentity> ();
			return netId != null ? netId.hasAuthority : false;
		}

		private void CleanNulls () {
			touchedObjects.RemoveAll (obj => obj == null);
		}

		private bool OwnCollider(Collider coll) {
			return colliders.Contains(coll);
		}
	}
}