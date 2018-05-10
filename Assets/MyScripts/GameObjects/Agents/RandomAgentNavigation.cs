using System;
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

		private NavMeshAgent agent;
		private Animator animator;
		[SyncVar (hook = "OnNewDestination")]
		private Vector3 destination;
		private Vector3 localDestination;
		private ARRelativeNetworkTransform arNetworkTransform;
		private GameObject target;

		void Start () {
			target = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			agent = GetComponent<NavMeshAgent> ();
			animator = GetComponent<Animator> ();
			arNetworkTransform = GetComponent<ARRelativeNetworkTransform> ();
		}

		void Update () {
			Debug.DrawRay (agent.destination, Vector3.up, Color.blue, Mathf.Infinity);
			Move ();
		}

		private void Move () {
			if (isServer && Arrived ()) {
				destination = TransformUtil.GetRandomPosition ();
			} else if (AgentLostPath ()) {
				OnNewDestination (destination);
			}

			animator.SetBool ("Moving", IsMoving ());
		}

		void OnNewDestination (Vector3 newDestination) {
			if (!isServer) {
				destination = newDestination;
			}

			localDestination = TransformUtil.Transform (TransformDirection.ServerToLocal, newDestination);
			var navMeshDestination = TransformUtil.ToNavMeshPosition (localDestination);
			if (agent != null) {
				var destinationSuccess = agent.SetDestination (navMeshDestination);
				target.transform.position = navMeshDestination;
			}
		}

		void SetAgentDestination (Vector3 destination) {
			
		}

		bool IsMoving () {
			return agent.velocity.magnitude > 0;
		}

		bool Arrived () {
			return agent.isOnNavMesh && !agent.pathPending &&
				Vector3.Distance (agent.transform.position, localDestination) <= agent.stoppingDistance;
		}

		private bool AgentLostPath () {
			return !agent.hasPath && !agent.pathPending &&
				Vector3.Distance (agent.transform.position, localDestination) > agent.stoppingDistance;
		}
	}
}