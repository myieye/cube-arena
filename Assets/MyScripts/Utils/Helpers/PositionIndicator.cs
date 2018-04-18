using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyPrefabs.Cubes;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
	public class PositionIndicator : MonoBehaviour {

		[SerializeField]
		private float minSize;
		[SerializeField]
		private float maxSize;
		[SerializeField]
		private float growRate;
		private BoxCollider boxCollider;
		private CubeStateManager cubeState;
		private Renderer rend;

		void Start () {
			boxCollider = transform.parent.GetComponentInChildren<BoxCollider> ();
			cubeState = GetComponentInParent<CubeStateManager> ();
			rend = GetComponent<Renderer> ();
		}

		void Update () {
			if (cubeState.InStates (CubeState.Drag, CubeState.Disallow)) {
				Show ();
			} else {
				Hide ();
			}
		}

		private void Show () {
			rend.enabled = true;

			var ray = new Ray (boxCollider.bounds.center, Vector3.down);
			RaycastHit hitInfo;
			if (Physics.Raycast (ray, out hitInfo, Mathf.Infinity, Layers.CubesAndTerrainMask)) {
				transform.position = hitInfo.point;
				transform.localScale = CalcScale (hitInfo.distance);
			}
			transform.rotation = Quaternion.identity;
		}

		private void Hide () {
			rend.enabled = false;
		}

		private Vector3 CalcScale (float dist) {
			var scale = Mathf.Clamp (dist * growRate, minSize, maxSize);
			return new Vector3 (scale, 1, scale);
		}
	}
}