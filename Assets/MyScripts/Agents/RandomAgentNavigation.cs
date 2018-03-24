﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

namespace CubeArena.Assets.MyScripts.Agents
{
	public class RandomAgentNavigation : NetworkBehaviour {

		public EnemySpawner enemyManager;
		private NavMeshAgent agent;
		private Animator animator;
		[SyncVar(hook="OnNewDestination")]
		private Vector3 destination;

		void Start () {
			agent = GetComponent<NavMeshAgent>();
			animator = GetComponent<Animator>();
		}
		
		void Update () {
			if (isServer && Arrived()) {
				destination = enemyManager.GetRandomPosition();
			}
			animator.SetBool("Moving", IsMoving());
		}

		void OnNewDestination(Vector3 _destination) {
			if (agent != null)
				agent.SetDestination(destination);
		}

		bool IsMoving() {
			return agent.velocity.magnitude > 0;
		}

		bool Arrived() {
			return 
				(!agent.pathPending) &&
				(agent.remainingDistance <= agent.stoppingDistance) &&
				(!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
		}
	}
}