using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.Utils.Constants;
using UnityEngine;
#if !UNITY_STANDALONE
using Vuforia;
#endif

namespace CubeArena.Assets.MyScripts.GameObjects.AR {
	public class ARManager:
#if !UNITY_STANDALONE// && !UNITY_EDITOR
		DefaultTrackableEventHandler
#else
	DummyTrackableEventHandler
#endif
	{

#if !UNITY_STANDALONE
		[SerializeField]
		private VuforiaBehaviour vuforiaBehaviour;
#endif
		[SerializeField]
		private GameObject world;
		private GameObject World {
			get {
				if (!world) {
					world = GameObject.Find ("World");
				}
				return world;
			}
		}
		private bool worldEnabled;
		private List<Renderer> rendererComponents;
		private List<ParticleSystem> particleSystemComponents;
		private List<CustomARObject> customObjects;
		public static ARManager Instance { get; private set; }

		public void Awake () {
			if (Instance) {
				Destroy (Instance);
			}
			Instance = this;
			rendererComponents = new List<Renderer> ();
			particleSystemComponents = new List<ParticleSystem> ();
			customObjects = new List<CustomARObject> ();
		}

		protected override void Start () {
			base.Start ();
			AddWorldComponents ();
			RefreshWorld ();

#if !UNITY_STANDALONE
			if (Settings.Instance.AREnabled) {
				VuforiaRuntime.Instance.InitVuforia ();
				VuforiaBehaviour.Instance.enabled = true;
			}
#endif
		}

		private void AddWorldComponents () {
			AddGameObjectComponents (World);
			foreach (var arObj in customObjects) {
				RemoveGameObjectComponents (arObj.gameObject);
			}
		}

		public void RegisterARObject (ARObject arObj) {
			arObj.gameObject.transform.parent = World.transform;
			AddGameObjectComponents (arObj.gameObject);
			RefreshWorld ();
		}

		public void RegisterCustomARObject (CustomARObject arObj) {
			customObjects.Add (arObj);
			RemoveGameObjectComponents (arObj.gameObject);
			RefreshWorld ();
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
			for (var i = 0; i < customObjects.Count; i++) {
				var arObj = customObjects[i];
				if (arObj == null || arObj.gameObject == null) {
					customObjects.RemoveAt (i--);
				} else {
					arObj.ARActive = worldEnabled;
				}
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