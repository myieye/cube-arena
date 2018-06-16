using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.PlayConfig.Rounds;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Settings;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace CubeArena.Assets.MyScripts.PlayConfig.Rounds {
	public class TimeManager : NetworkBehaviour {

		[SerializeField]
		private Text clock;
		[SerializeField]
		private GameObject practiceModeIndicator;
		[SyncVar (hook = "OnRoundTimeRemainingChanged")]
		private int roundTimeRemaining_S;
		private const string TickClock_Method = "TickClock";
		private RoundOverListener roundOverListener;

		void OnEnable () {
			Clear ();
		}

		void OnDisable () {
			if (Settings.Instance.ForceUserStudySettings) {
				Debug.Log ("Time on disable: " + GetTimeText ());
			}

			Clear ();
			OnRoundTimeRemainingChanged (roundTimeRemaining_S);
		}

		[Server]
		public void StartRound (float roundLength, float roundDelay, RoundOverListener roundOverListener, bool practiceMode) {
			this.roundOverListener = roundOverListener;

			Clear ();
			RpcClear (false);

			StartCoroutine (DelayUtil.Do (roundDelay,
				() => {
					roundTimeRemaining_S = Mathf.CeilToInt (roundLength);
					InvokeRepeating (TickClock_Method, 1, 1);
					RpcShow (practiceMode);
				}));
		}

		[ClientRpc]
		public void RpcClear (bool clearOnServer) {
			Debug.Log ("RpcClear: " + clearOnServer);
			if (!isServer || clearOnServer) {
				Clear ();
				Debug.Log ("clearing");
			}
		}

		private void Clear () {
			CancelInvoke (TickClock_Method);
			Debug.Log ("canceled invoked");
			roundTimeRemaining_S = 0;
			if (clock) {
				clock.enabled = false;
			}
			if (practiceModeIndicator) {
				practiceModeIndicator.SetActive (false);
			}
		}

		[ClientRpc]
		private void RpcShow (bool practiceMode) {
			clock.enabled = true;
			practiceModeIndicator.SetActive (practiceMode);
		}

		[Server]
		private void TickClock () {
			roundTimeRemaining_S -= 1;
			if (roundTimeRemaining_S <= 0) {
				RpcClear (true);
				roundOverListener.OnRoundOver ();
			}
		}

		private void OnRoundTimeRemainingChanged (int roundTimeRemaining) {
			roundTimeRemaining_S = roundTimeRemaining;
			if (clock) {
				clock.text = GetTimeText ();
			}
		}

		private string GetTimeText () {
			TimeSpan time = TimeSpan.FromSeconds (roundTimeRemaining_S);
			return string.Format ("{0}:{1:D2}", time.Minutes, time.Seconds);
		}
	}
}