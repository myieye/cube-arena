using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.Utils.Constants;
using CubeArena.Assets.MyScripts.Utils.Settings;
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
		private List<Renderer> rendererComponents;
		private List<ParticleSystem> particleSystemComponents;
		private List<CustomARObject> customObjects;
		public static ARManager Instance { get; private set; }
		public static bool WorldEnabled { get; private set; }

		public void Awake () {
			if (Instance) {
				Destroy (Instance);
			}
			WorldEnabled = !Settings.Instance.AREnabled;
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

		public void RegisterCustomARObject (CustomARObject arObj, bool handleManually = true) {
			customObjects.Add (arObj);
			if (handleManually) {
				RemoveGameObjectComponents (arObj.gameObject);
			}
			RefreshWorld ();
		}

		protected override void OnTrackingFound () {
#if UNITY_WSA// && !UNITY_EDITOR
			if (CustomNetworkManager.IsServer) {
				var behaviour = FindObjectOfType<Vuforia.ImageTargetBehaviour> ();
				var t = behaviour.Trackable;
				Debug.Log (t.Name);
				var it = t as Vuforia.ImageTarget;
				it.StartExtendedTracking ();
				Debug.Log ("Starting extended tracking");
			}
#endif
			WorldEnabled = true;
			RefreshWorld ();
		}

		protected override void OnTrackingLost () {
			WorldEnabled = false;
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
						component.enabled = WorldEnabled;
					}
				}

				// Set particle systems:
				for (var i = 0; i < particleSystemComponents.Count; i++) {
					var component = particleSystemComponents[i];
					if (!component) {
						particleSystemComponents.RemoveAt (i--);
					} else if (WorldEnabled && !component.isPlaying) {
						component.Play ();
					} else if (!WorldEnabled) {
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
					arObj.ARActive = WorldEnabled;
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