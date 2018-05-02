using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.GameObjects.Agents {
	public class RandomAgentNavigation : NetworkBehaviour {

		public EnemySpawner enemySpawner;
		private NavMeshAgent agent;
		private Animator animator;
		[SyncVar (hook = "OnNewDestination")]
		private Vector3 destination;

		void Start () {
			agent = GetComponent<NavMeshAgent> ();
			animator = GetComponent<Animator> ();
		}

		void Update () {
			Debug.DrawRay(destination, Vector3.up, Color.blue, Mathf.Infinity);

			if (isServer && Arrived ()) {
				destination = enemySpawner.GetRandomPosition ();
			}

			animator.SetBool ("Moving", IsMoving ());
		}

		void OnNewDestination (Vector3 newDestination) {
			if (agent != null) {
				var origin = GameObject.Find (Names.Ground);
				agent.SetDestination (TransformUtil.Transform (TransformDirection.ServerToLocal, newDestination));
			}
		}

		bool IsMoving () {
			return agent.velocity.magnitude > 0;
		}

		bool Arrived () {
			return //(!agent.pathPending) &&
			(agent.remainingDistance <= agent.stoppingDistance) /*&&
			(!agent.hasPath || agent.velocity.sqrMagnitude == 0f)*/;
		}
	}
}