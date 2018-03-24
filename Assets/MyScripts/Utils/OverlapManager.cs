using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Utils
{
	public class OverlapManager : MonoBehaviour {

		public List<string> tagList = new List<string>();
		public List<string> ignoreList = new List<string> {"UI"};
		public List<GameObject> touchedObjects = new List<GameObject>();
		
		public virtual void OnTriggerEnter(Collider col) {
			if (HasMatchingTag(col.gameObject)) {
				touchedObjects.Add(col.gameObject);
			}
		}
		
		public virtual void OnTriggerExit(Collider col) {
			if (HasMatchingTag(col.gameObject)) {
				touchedObjects.Remove(col.gameObject);
			}
		}

		public bool HasMatchingTag(GameObject obj) {
			if (ignoreList.Contains(obj.tag))
				return false;
			return !tagList.Any() || tagList.Contains(obj.tag);
		}

		public GameObject GetClosest() {
			GameObject closest;
			GetClosest(out closest);
			return closest;
		}

		public GameObject GetLocalClosest() {
			GameObject closest;
			GetLocalClosest(out closest);
			return closest;
		}

		public bool GetClosest(out GameObject closest) {
			return GetClosest(out closest, false);
		}

		public bool GetLocalClosest(out GameObject closest) {
			return GetClosest(out closest, true);
		}

		private bool GetClosest(out GameObject closest, bool localOnly) {
			if (!HasOverlap(localOnly)) {
				closest = null;
				return false;
			}
			
			var minDist = float.MaxValue;
			closest = localOnly ? touchedObjects.First(IsLocal) : touchedObjects.First();
			foreach (var gameObj in touchedObjects) {
				if (!localOnly || IsLocal(gameObj)) {
					var dist = Vector3.Distance(gameObj.transform.position, transform.position);
					if (dist < minDist) {
						minDist = dist;
						closest = gameObj;
					}
				}
			}
			return true;
		}

		public bool HasOverlapWith(GameObject obj) {
			return obj != null && touchedObjects.Contains(obj);
		}

		public bool HasOverlapBelow() {
			return touchedObjects.Any(IsBelow);
		}

		public bool HasOverlap() {
			return touchedObjects.Any();
		}

		public bool HasLocalOverlap() {
			return touchedObjects.Any(IsLocal);
		}

		private bool HasOverlap(bool localOnly) {
			return localOnly ? HasLocalOverlap() : HasOverlap();
		}

		private bool IsBelow(GameObject obj) {
			return obj.transform.position.y < this.transform.position.y;
		}

		private bool IsLocal(GameObject obj) {
			var netId = obj.GetComponent<NetworkIdentity>();
			return netId != null ? netId.hasAuthority : false;
		}
	}
}