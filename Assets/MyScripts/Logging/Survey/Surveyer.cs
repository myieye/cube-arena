using System;
using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Logging.DAL;
using CubeArena.Assets.MyScripts.Logging.DAL.Models.Answers;
using CubeArena.Assets.MyScripts.Logging.Survey.Models;
using CubeArena.Assets.MyScripts.PlayConfig.Players;
using CubeArena.Assets.MyScripts.Utils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.UI;
using NetworkPlayer = CubeArena.Assets.MyScripts.PlayConfig.Players.NetworkPlayer;
using CubeArena.Assets.MyPrefabs.Player;

namespace CubeArena.Assets.MyScripts.Logging.Survey {
    [RequireComponent (typeof (RatingQuestionAsker), typeof (WeightQuestionAsker))]
    public class Surveyer : NetworkBehaviour {

        [SerializeField]
        private GameObject surveyContainer;
        [SerializeField]
        private RectTransform questionContainer;
        [SerializeField]
        private GameObject ratingQuestionContainer;
        [SerializeField]
        private GameObject weightQuestionContainer;
        [SerializeField]
        private Text questionCounter;

        [SerializeField]
        private GameObject surveyCursor;

        // Server ---
        private SurveyFinishedListener surveyFinishedListener;
        private int totalClients;
        private int clientsFinished;
        // ---

        // Client ---
        private IList<Question> questions;
        public int CurrentQuestionI { get; private set; }
        public bool SurveyStarted { get; private set; }
        private int playerId;

        private RatingQuestionAsker ratingQuestionAsker;
        private WeightQuestionAsker weightQuestionAsker;
        public QuestionAsker CurrentQuestionAsker {
            get {
                if (!SurveyStarted) {
                    return null;
                } else if (CurrentQuestion is RatingQuestion) {
                    return ratingQuestionAsker;
                } else {
                    return weightQuestionAsker;
                }
            }
        }
        public Question CurrentQuestion {
            get {
                return questions[CurrentQuestionI];
            }
        }
        // ---

        private void Start () {
#if UNITY_ANDROID
            surveyContainer.GetComponent<RectTransform> ().localScale = new Vector3 (2.5f, 2.5f, 1);
#elif UNITY_WSA && !UNITY_EDITOR
            surveyContainer.GetComponent<RectTransform> ().localScale = new Vector3 (0.3f, 0.3f, 0.3f);
#endif
            ratingQuestionAsker = GetComponent<RatingQuestionAsker> ();
            weightQuestionAsker = GetComponent<WeightQuestionAsker> ();
        }

        public void DoSurvey (List<NetworkPlayer> players, SurveyFinishedListener surveyFinishedListener) {
            this.surveyFinishedListener = surveyFinishedListener;
            totalClients = players.Count;
            clientsFinished = 0;

            foreach (var player in players) {
                TargetDoSurvey (player.DeviceConfig.Device.Connection, player.PlayerId);
            }
        }

        [TargetRpc]
        public void TargetDoSurvey (NetworkConnection target, int playerId) {            
#if UNITY_WSA && !UNITY_EDITOR
            surveyCursor.SetActive (true);
#endif

            this.playerId = playerId;
            SurveyStarted = true;
            questions = QuestionService.GetShuffledQuestions ();
            CurrentQuestionI = 0;
            surveyContainer.SetActive (true);

            AskCurrQuestion ();
        }

        public void OnNext () {
            if (!SurveyStarted) return;

            Assert.IsTrue (CurrentQuestionAsker.HasValidAnswer ());

            SaveCurrentQuestion ();

            if (CurrentQuestionI + 1 == questions.Count) {
                OnSurveyComplete ();
            } else {
                CurrentQuestionI++;
                AskCurrQuestion ();
            }
        }

        public void OnBack () {
            if (!SurveyStarted) return;

            Assert.IsTrue (CurrentQuestionI > 0);

            CurrentQuestionI--;
            AskCurrQuestion ();
        }

        private void SaveCurrentQuestion () {
            if (CurrentQuestion is RatingQuestion) {
                var answer = ratingQuestionAsker.GetRatingAnswer ();
                ServerLogger.LocalInstance.CmdLogRatingAnswer (playerId, answer, answer.Id);
            } else {
                var answer = weightQuestionAsker.GetWeightAnswer ();
                ServerLogger.LocalInstance.CmdLogWeightAnswer (playerId, answer, answer.Id);
            }
        }

        private void OnSurveyComplete () {            
#if UNITY_WSA && !UNITY_EDITOR
            surveyCursor.SetActive (false);
#endif

            surveyContainer.SetActive (false);
            SurveyStarted = false;
            ServerBridge.LocalInstance.CmdOnSurveyComplete (playerId);
        }

        public void OnClientCompletedSurvey (int playerId) {
            clientsFinished++;
            if (clientsFinished == totalClients) {
                surveyFinishedListener.OnSurveyFinished ();
            }
        }

        private void AskCurrQuestion () {
            questionCounter.text = String.Format ("{0} / {1}", CurrentQuestionI + 1, questions.Count);

            ratingQuestionContainer.SetActive (CurrentQuestion is RatingQuestion);
            weightQuestionContainer.SetActive (!(CurrentQuestion is RatingQuestion));

            if (CurrentQuestion is RatingQuestion) {
                ratingQuestionAsker.AskRatingQuestion (CurrentQuestion as RatingQuestion);
            } else {
                weightQuestionAsker.AskRatingQuestion (CurrentQuestion as WeightQuestion, OnNext);
            }
        }
    }
}