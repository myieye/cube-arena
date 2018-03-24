using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Test
{
	public class Movable : MonoBehaviour {

		public float speed = 30;

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			var currSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * 2 : speed;

			if(Input.GetKey(KeyCode.RightArrow))
			{
				transform.Translate(new Vector3(currSpeed * Time.deltaTime,0,0));
			}
			if(Input.GetKey(KeyCode.LeftArrow))
			{
				transform.Translate(new Vector3(-currSpeed * Time.deltaTime,0,0));
			}
			if(Input.GetKey(KeyCode.DownArrow))
			{
				transform.Translate(-transform.forward * currSpeed * Time.deltaTime, Space.World);
			}
			if(Input.GetKey(KeyCode.UpArrow))
			{
				transform.Translate(transform.forward * currSpeed * Time.deltaTime, Space.World);
			}
			
			if(Input.GetKey(KeyCode.D))
			{
				transform.Rotate(new Vector3(0,currSpeed * Time.deltaTime,0), Space.World);
			}
			if(Input.GetKey(KeyCode.A))
			{
				transform.Rotate(new Vector3(0,-currSpeed * Time.deltaTime,0), Space.World);
			}
			if(Input.GetKey(KeyCode.S))
			{
				transform.Rotate(new Vector3(currSpeed * Time.deltaTime,0,0));
			}
			if(Input.GetKey(KeyCode.W))
			{
				transform.Rotate(new Vector3(-currSpeed * Time.deltaTime,0,0));
			}
		}
	}
}