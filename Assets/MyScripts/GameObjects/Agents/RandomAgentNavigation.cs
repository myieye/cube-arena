using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.GameObjects.Agents {

	public class RandomAgentNavigation : NetworkBehaviour {

		[HideInInspector]
		public EnemySpawner enemySpawner;
		private NavMeshAgent agent;
		private Animator animator;
		[SyncVar (hook = "OnNewDestination")]
		private Vector3 destination;
		private ARRelativeNetworkTransform arNetworkTransform;

		void Start () {
			agent = GetComponent<NavMeshAgent> ();
			animator = GetComponent<Animator> ();
			arNetworkTransform = GetComponent<ARRelativeNetworkTransform> ();
		}

		void Update () {
			//Debug.Log("Update: " + transform.position);
			Debug.DrawRay (agent.destination, Vector3.up, Color.blue, Mathf.Infinity);
			Move ();
		}

		private void Move () {
			if (isServer && Arrived ()) {
				destination = enemySpawner.GetRandomPosition ();
			}
			animator.SetBool ("Moving", IsMoving ());
		}

		void OnNewDestination (Vector3 newDestination) {
			newDestination = TransformUtil.Transform (TransformDirection.ServerToLocal, newDestination);
			var navMeshDestination = enemySpawner.ToNavMeshPosition (newDestination);
			if (agent != null) {
				agent.SetDestination (navMeshDestination);
			}
		}

		bool IsMoving () {
			return agent.velocity.magnitude > 0;
		}

		bool Arrived () {
			return agent.isOnNavMesh && //(!agent.pathPending) &&
				(agent.remainingDistance <= agent.stoppingDistance)
			/*&&
			(!agent.hasPath || agent.velocity.sqrMagnitude == 0f)*/
			;
		}
	}
}