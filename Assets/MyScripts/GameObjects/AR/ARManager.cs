using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.GameObjects.AR {
	public class ARManager:
#if !UNITY_STANDALONE && !UNITY_EDITOR
		DefaultTrackableEventHandler
#else
	DummyTrackableEventHandler
#endif
	{

		public GameObject world;
		bool worldEnabled;
		List<Renderer> rendererComponents;
		List<ParticleSystem> particleSystemComponents;
		List<ARObject> arObjects;

		protected override void Start () {
			base.Start ();
			rendererComponents = new List<Renderer> ();
			particleSystemComponents = new List<ParticleSystem> ();
			arObjects = new List<ARObject> ();
			//colliderComponents = new List<Collider>();
			//canvasComponents = new List<Canvas>();
			AddGameObjectComponents (gameObject);
			RefreshWorld ();
		}

		public void AddGameObjectToWorld (GameObject obj) {
			obj.transform.parent = world.transform;
			AddGameObjectComponents (obj);
			RefreshWorld ();
		}

		public void AddARObjectToWorld (ARObject arObj) {
			arObjects.Add (arObj);
			RemoveGameObjectComponents (arObj.gameObject);
		}

		protected override void OnTrackingFound () {
			worldEnabled = true;
			RefreshWorld ();
		}

		protected override void OnTrackingLost () {
			worldEnabled = false;
			RefreshWorld ();
		}

		private void RefreshWorld () {
			if (Settings.Instance.AREnabled) {
				// Set rendering:
				for (var i = 0; i < rendererComponents.Count; i++) {
					var component = rendererComponents[i];
					if (!component) {
						rendererComponents.RemoveAt (i--);
					} else {
						component.enabled = worldEnabled;
					}
				}

				// Set particle systems:
				for (var i = 0; i < particleSystemComponents.Count; i++) {
					var component = particleSystemComponents[i];
					if (!component) {
						particleSystemComponents.RemoveAt (i--);
					} else if (worldEnabled && !component.isPlaying) {
						component.Play ();
					} else if (!worldEnabled) {
						component.Stop (true, ParticleSystemStopBehavior.StopEmittingAndClear);
					}
				}
			}

			// Set Custom ARObjects:
			foreach (var arObj in arObjects) {
				arObj.SetArActive (!Settings.Instance.AREnabled || worldEnabled);
			}
		}

		private void AddGameObjectComponents (GameObject obj) {
			rendererComponents.AddRange (obj.GetComponentsInChildren<Renderer> (true));
			particleSystemComponents.AddRange (obj.GetComponentsInChildren<ParticleSystem> (true));
		}

		private void RemoveGameObjectComponents (GameObject obj) {
			foreach (var component in obj.GetComponentsInChildren<Renderer> (true)) {
				rendererComponents.Remove (component);
			}
			foreach (var component in obj.GetComponentsInChildren<ParticleSystem> (true)) {
				particleSystemComponents.Remove (component);
			}
		}
	}
}