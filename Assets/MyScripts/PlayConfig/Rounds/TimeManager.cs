using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.PlayConfig.Rounds;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace CubeArena.Assets.MyScripts.PlayConfig.Rounds {
	public class TimeManager : NetworkBehaviour {

		public Text clock;
		[SyncVar (hook = "OnRoundTimeRemainingChanged")]
		private int roundTimeRemaining_S;
		private const string TickClock_Method = "TickClock";
		private RoundOverListener roundOverListener;

		public void StartRound (float roundLength, RoundOverListener roundOverListener) {
			if (isServer) {
				this.roundOverListener = roundOverListener;
				roundTimeRemaining_S = Mathf.CeilToInt (roundLength * 60f);
				CancelInvoke (TickClock_Method);
				InvokeRepeating (TickClock_Method, 0, 1);
			}
		}

		private void TickClock () {
			if (isServer) {
				roundTimeRemaining_S -= 1;
				if (roundTimeRemaining_S <= 0) {
					CancelInvoke (TickClock_Method);
					roundOverListener.OnRoundOver ();
				}
			}
		}

		private void OnRoundTimeRemainingChanged (int roundTimeRemaining) {
			TimeSpan time = TimeSpan.FromSeconds (roundTimeRemaining);
			var clockTime = string.Format ("{0}:{1:D2}", time.Minutes, time.Seconds);
			clock.text = clockTime;
		}
	}
}