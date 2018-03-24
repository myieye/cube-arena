using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Utils
{
	public class Explodable : MonoBehaviour {

		private Rigidbody rigidBody;

		// Use this for initialization
		void Start () {
			rigidBody = GetComponent<Rigidbody>();
		}
		
		// Update is called once per frame
		void Update () {
			if (Input.GetMouseButtonDown(0)) {
				Debug.Log("Explosion");
				rigidBody.AddExplosionForce(300, transform.position + Vector3.left * 2,
					500.0f, 500.0f, ForceMode.Acceleration);
			}
		}
	}
}