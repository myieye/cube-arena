using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;

namespace CubeArena.Assets.MyPrefabs.Enemies {

	public class EnemySlower : MonoBehaviour {

		[SerializeField]
		private float penaltyPerSlower;
		[SerializeField]
		private float minSpeed;
		private Animator animator;
		private float maxSpeed;
		private OverlapManager overlapManager;
		private int prevCount;

		void Start () {
			overlapManager = GetComponent<OverlapManager> ();
			animator = GetComponent<Animator> ();
			prevCount = 0;
			maxSpeed = animator.GetFloat("Speed");
		}

		void Update () {
			var newCount = overlapManager.GetCount (Tags.Slower);
			if (prevCount != newCount) {
				prevCount = newCount;
				var newSpeed = Mathf.Max (minSpeed, maxSpeed - (newCount * penaltyPerSlower));
				animator.SetFloat ("Speed", newSpeed);
			}
		}
	}
}