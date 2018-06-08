using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.PlayConfig.Rounds;
using CubeArena.Assets.MyScripts.Utils;
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
			Clear ();
			OnRoundTimeRemainingChanged (roundTimeRemaining_S);
		}

		[Server]
		public void StartRound (float roundLength, float roundDelay, RoundOverListener roundOverListener, bool practiceMode) {
			this.roundOverListener = roundOverListener;

			RpcClear ();

			StartCoroutine (DelayUtil.Do (roundDelay,
				() => {
					roundTimeRemaining_S = Mathf.CeilToInt (roundLength * 60f);
					InvokeRepeating (TickClock_Method, 1, 1);
					clock.enabled = true;
					RpcShowHidePracticeModeIndicator (practiceMode);
				}));
		}

		[ClientRpc]
		public void RpcClear () {
			Clear ();
		}

		private void Clear () {
			CancelInvoke (TickClock_Method);
			roundTimeRemaining_S = 0;
			if (clock) {
				clock.enabled = false;
			}
			if (practiceModeIndicator) {
				practiceModeIndicator.SetActive (false);
			}
		}

		[ClientRpc]
		private void RpcShowHidePracticeModeIndicator (bool practiceMode) {
			practiceModeIndicator.SetActive (practiceMode);
		}

		[Server]
		private void TickClock () {
			roundTimeRemaining_S -= 1;
			if (roundTimeRemaining_S <= 0) {
				RpcClear ();
				roundOverListener.OnRoundOver ();
			}
		}

		private void OnRoundTimeRemainingChanged (int roundTimeRemaining) {
			if (clock) {
				TimeSpan time = TimeSpan.FromSeconds (roundTimeRemaining);
				var clockTime = string.Format ("{0}:{1:D2}", time.Minutes, time.Seconds);
				clock.text = clockTime;
			}
		}
	}
}