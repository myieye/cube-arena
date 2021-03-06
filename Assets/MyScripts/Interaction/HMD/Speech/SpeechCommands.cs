﻿using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.PlayConfig.UIModes;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Settings;
using CubeArena.Assets.MyScripts.Utils.TransformUtils;
using CubeArena.Assets.MyScripts.Utils.UIUtils;
using UnityEngine;
using UnityEngine.Networking;

public class SpeechCommands : MonoBehaviour {

	[SerializeField]
	private GameObject marker;
	[SerializeField]
	private ParticleSystem groundFlame;

	private void Start () {
		groundFlame.gameObject.SetActive (false);
	}

	public void ShiftWorldToCameraPosition () {
		var blue = Color.blue;
		blue.a = 0.4f;
		var red = Color.red;
		red.a = 0.4f;

		Instantiate (marker, Vector3.zero, Quaternion.identity)
			.GetComponent<Renderer> ().material.color = blue;

		var world = GameObject.Find (Names.ARWorld);
		var camera = Camera.main.gameObject;

		var newCameraPos = world.transform.position * -1;
		var newWorldPos = camera.transform.position;

		var newCameraRot = Quaternion.Inverse (world.transform.rotation);
		var newWorldRot = camera.transform.rotation;

		world.transform.position = newWorldPos;
		world.transform.rotation = newWorldRot;

		camera.transform.position = newCameraPos;
		camera.transform.rotation = newCameraRot;

		Instantiate (marker, Vector3.zero, Quaternion.identity)
			.GetComponent<Renderer> ().material.color = red;
	}

	public void DrawMarkerAtZero () {
		Instantiate (marker, Vector3.zero, Quaternion.identity);
	}

	public void StartHost () {
		FindObjectOfType<NetworkManager> ().StartHost ();
	}

	public void Disconnect () {
		FindObjectOfType<NetworkManager> ().StopHost ();
	}

	public void ConnectClient () {
		NetworkManager.singleton.networkAddress = Settings.Instance.ServerIp;
		FindObjectOfType<NetworkManager> ().StartClient ();
	}

	public void SetUIMode (int uiMode) {
		UIModeManager.Instance<UIModeManager> ().OnUIModeChanged (uiMode);
	}

	public void PrintCameraPosition () {
		Debug.Log ("Camera Pos: " + Camera.main.transform.position);
	}

	public void PrintWorldInfo () {
		Debug.Log (
			". Ground Pos: " + TransformUtil.World.position +
			". Ground radius: " + TransformUtil.LocalRadius);
	}

	public void ToggleGroundFlame () {
		groundFlame.gameObject.SetActive (!groundFlame.gameObject.activeSelf);
	}

	public void CancelNetworkSearch () {
		CustomNetworkDiscovery.Instance.StopBroadcasting ();
	}

	public void StartNetworkSearch () {
		CustomNetworkDiscovery.Instance.StartBroadcasting ();
	}

	public void SetIpAddress () {
		IPAddressSetter.Instance.OpenIPAddressSetter ();
	}
}