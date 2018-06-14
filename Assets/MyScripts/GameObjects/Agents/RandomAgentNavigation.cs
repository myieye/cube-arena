using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.GameObjects.AR;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.GameObjects.Agents {

	public class RandomAgentNavigation : NetworkBehaviour {

		[SerializeField]
		private GameObject targetPrefab;
		private GameObject target;
		private NavMeshAgent agent;
		private Animator animator;
		[SyncVar (hook = "OnNewDestination")]
		private Vector3 destination;
		private Vector3 localDestination;
		[SyncVar]
		private bool movingOnServer;
		[HideInInspector]
		[SyncVar]
		public bool movingEnemy;

		void Start () {
			agent = GetComponent<NavMeshAgent> ();
			animator = GetComponent<Animator> ();
		}

		public override void OnStartClient () {
			Init ();
		}

		private void Init () {
			enabled = movingEnemy;

			if (movingEnemy && !target) {
				target = Instantiate (targetPrefab);
			}

			if (movingEnemy && isServer) {
				destination = transform.position;
			}
		}

		void OnDisable () {
			if (target) {
				Destroy (target);
			}
		}

		void Update () {
			Debug.DrawRay (agent.destination, Vector3.up, Color.blue, Mathf.Infinity);
			Move ();
			if (target) {
				target.SetActive (ARManager.WorldEnabled);
			}
		}

		private void Move () {
			if (isServer && Arrived ()) {
				destination = TransformUtil.GetRandomPosition ();
			} else if (AgentLostPath ()) {
				//#if UNITY_WSA && !UNITY_EDITOR
				//				transform.LookAt ()
				//#else
				OnNewDestination (destination);
				//#endif
			}

			animator.SetBool ("Moving", IsMoving ());
		}

		void OnNewDestination (Vector3 newDestination) {
			if (!isServer) {
				destination = newDestination;
			}

			localDestination = newDestination.ToLocal ();
			var navMeshDestination = TransformUtil.ToNavMeshPosition (localDestination);
			if (navMeshDestination.HasValue && agent != null && agent.isOnNavMesh) {
				agent.SetDestination (navMeshDestination.Value);
			}

			if (target) {
				target.transform.rotation = TransformUtil.World.rotation;
				target.transform.position = navMeshDestination.Value;
			}
		}

		bool IsMoving () {
			var moving = agent.velocity.magnitude > 0;

			if (isServer) {
				movingOnServer = moving;
				return moving;
			} else {
				return moving || (movingOnServer && !AtDestination ());
			}
		}

		bool Arrived () {
			return agent.isOnNavMesh && !agent.pathPending && AtDestination ();
		}

		private bool AgentLostPath () {
			return agent.isOnNavMesh && !agent.hasPath && !agent.pathPending && !AtDestination ();
		}

		private bool AtDestination () {
			return Vector3.Distance (agent.transform.position, localDestination) <= agent.stoppingDistance;
		}
	}
}