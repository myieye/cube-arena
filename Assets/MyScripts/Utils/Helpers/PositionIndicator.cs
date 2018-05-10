using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyPrefabs.Cubes;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils.Helpers {
	public class PositionIndicator : CustomARObject {

		[SerializeField]
		private float minSize;
		[SerializeField]
		private float maxSize;
		[SerializeField]
		private float growRate;
		private BoxCollider boxCollider;
		private CubeStateManager cubeState;
		private Renderer rend;

		public override void Start () {
			base.Start ();
			boxCollider = transform.parent.GetComponentInChildren<BoxCollider> ();
			cubeState = GetComponentInParent<CubeStateManager> ();
			rend = GetComponent<Renderer> ();
		}

		void Update () {
			if (ShouldBeShown()) {
				TryShow ();
			} else {
				Hide ();
			}
		}

		private void TryShow () {
			var ray = new Ray (boxCollider.bounds.center, TransformUtil.World.up * -1);
			RaycastHit hitInfo;
			if (Physics.Raycast (ray, out hitInfo, Mathf.Infinity, Layers.CubesAndTerrainMask)) {
				transform.position = hitInfo.point;
				transform.rotation = TransformUtil.World.rotation;
				transform.localScale = CalcScale (hitInfo.distance);
				rend.enabled = true;
			} else {
				rend.enabled = false;
			}
		}

		private void Hide () {
			rend.enabled = false;
		}

		private bool ShouldBeShown() {
			return ARActive && cubeState.InStates (CubeState.Drag, CubeState.Disallow);
		}

		private Vector3 CalcScale (float dist) {
			var scale = Mathf.Clamp (dist * growRate, minSize, maxSize);
			return new Vector3 (scale, 1, scale);
		}
	}
}