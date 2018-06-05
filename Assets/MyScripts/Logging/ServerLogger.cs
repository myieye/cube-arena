using System;
using System.Collections;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Logging.DAL;
using CubeArena.Assets.MyScripts.Logging.DAL.Models;
using CubeArena.Assets.MyScripts.Logging.DAL.Models.Answers;
using CubeArena.Assets.MyScripts.Network;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using CubeArena.Assets.MyScripts.PlayConfig.Rounds;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace CubeArena.Assets.MyScripts.Logging {
	public class ServerLogger : NetworkBehaviour {

		private DataService dataService;
		private RoundManager roundManager;

		private PlayerId _playerId;
		private int PlayerId {
			get {
				if (!_playerId) {
					_playerId = GetComponent<PlayerId> ();
				}
				return _playerId.Id;
			}
		}

		void Start () {
			if (isServer) {
				dataService = DataService.Instance;
				roundManager = FindObjectOfType<RoundManager> ();
			}
		}

		[Command]
		public void CmdLog (string msg) {
			dataService.Log (msg);
		}

		[Command]
		public void CmdLogMove (Move move, float time) {
			move.Time = time;
			dataService.SaveMove (AddDbInfo (move));
		}

		[Command]
		public void CmdLogRotation (Rotation rotation, float time) {
			rotation.Time = time;
			dataService.SaveRotation (AddDbInfo (rotation));
		}

		[Command]
		public void CmdLogSelectionAction (SelectionAction selectionAction) {
			dataService.SaveSelectionAction (AddDbInfo (selectionAction));
		}

		[Command]
		public void CmdLogSelection (Selection selection, float time) {
			selection.Time = time;
			dataService.SaveSelecion (AddDbInfo (selection));
		}

		[Command]
		public void CmdLogPlacement (Placement placement) {
			dataService.SavePlacement (AddDbInfo (placement));
		}

		[Command]
		public void CmdLogKill (Kill kill, Assist[] assists) {
			foreach (var a in assists) {
				AddDbInfo (a, false);
			}
			dataService.SaveKill (AddDbInfo (kill, false), new List<Assist> (assists));
		}

		[Command]
		public void CmdLogAreaInteraction (AreaInteraction areaInteraction, float time) {
			areaInteraction.Time = time;
			dataService.SaveAreaInteraction (AddDbInfo (areaInteraction));
		}

		[Command]
		public void CmdLogCloudMeasurement (CloudMeasurement cloudMeasurement) {
			dataService.SaveCloudMeasurement (AddDbInfo (cloudMeasurement));
		}

		private T AddDbInfo<T> (T measurement, bool setPlayerRoundId = true) where T : Measurement {
			if (setPlayerRoundId) {
				measurement.PlayerRoundId = PlayerManager.Instance.GetPlayerRoundId (PlayerId);
			}
			measurement.PracticeMode = roundManager.InPracticeMode;
			return measurement;
		}

		public void CmdLogWeightAnswer (WeightAnswer answer, int id) {
			answer.PlayerRoundId = PlayerManager.Instance.GetPlayerRoundId (PlayerId);
			answer.Id = id;
			dataService.SaveWeightAnswer (answer);
		}

		public void CmdLogRatingAnswer (RatingAnswer answer, int id) {
			answer.PlayerRoundId = PlayerManager.Instance.GetPlayerRoundId (PlayerId);
			answer.Id = id;
			dataService.SaveRatingAnswer (answer);
		}
	}
}