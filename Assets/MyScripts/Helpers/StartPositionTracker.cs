using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPositionTracker : MonoBehaviour {

	public Vector3 StartPosition { get; private set; }
	
	void Start () {
		StartPosition = transform.position;
	}
}
