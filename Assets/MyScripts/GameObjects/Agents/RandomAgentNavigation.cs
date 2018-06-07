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
			if (!movingEnemy) {
				enabled = false;
			} else if (isServer) {
				destination = transform.position;
			}
		}

		void OnEnable () {
			if (!movingEnemy) {
				enabled = false;
			} else if (!target) {
				target = Instantiate (targetPrefab);
				target.SetActive (false);
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
			Debug.Log ("OnNewDestination: " + newDestination);

			if (!isServer) {
				destination = newDestination;
			}

			localDestination = newDestination.ToLocal ();
			var navMeshDestination = TransformUtil.ToNavMeshPosition (localDestination);
			if (navMeshDestination.HasValue && agent != null) {
				agent.SetDestination (navMeshDestination.Value);
			}

			if (target) {
				target.SetActive (true);
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