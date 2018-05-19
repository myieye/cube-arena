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

		[Server]
		public void StartRound (float roundLength, float roundDelay, RoundOverListener roundOverListener, bool practiceMode) {
			this.roundOverListener = roundOverListener;

			roundTimeRemaining_S = Mathf.CeilToInt (roundLength * 60f);
			CancelInvoke (TickClock_Method);
			StartCoroutine (DelayUtil.Do (roundDelay,
				() => InvokeRepeating (TickClock_Method, 1, 1)));
				
			RpcShowHidePracticeModeIndicator (practiceMode);
		}

		[ClientRpc]
		private void RpcShowHidePracticeModeIndicator (bool practiceMode) {
			practiceModeIndicator.SetActive (practiceMode);
		}

		[Server]
		private void TickClock () {
			roundTimeRemaining_S -= 1;
			if (roundTimeRemaining_S <= 0) {
				CancelInvoke (TickClock_Method);
				roundOverListener.OnRoundOver ();
			}
		}

		private void OnRoundTimeRemainingChanged (int roundTimeRemaining) {
			TimeSpan time = TimeSpan.FromSeconds (roundTimeRemaining);
			var clockTime = string.Format ("{0}:{1:D2}", time.Minutes, time.Seconds);
			clock.text = clockTime;
		}
	}
}