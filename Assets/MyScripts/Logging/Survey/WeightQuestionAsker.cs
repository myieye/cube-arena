using System;
using CubeArena.Assets.MyScripts.Logging.DAL.Models.Answers;
using CubeArena.Assets.MyScripts.Logging.Survey.Models;
using CubeArena.Assets.MyScripts.Utils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace CubeArena.Assets.MyScripts.Logging.Survey {
    public class WeightQuestionAsker : MonoBehaviour, QuestionAsker {

        [SerializeField]
        private ToggleGroup toggleGroup;
        [SerializeField]
        private Toggle optionOneToggle;
        [SerializeField]
        private Toggle optionTwoToggle;

        private WeightQuestion currQ;
        private Action then;
        private bool answered;

        private void Start () {
            optionOneToggle.onValueChanged.AddListener (OnOptionSelected);
            optionTwoToggle.onValueChanged.AddListener (OnOptionSelected);
        }

        private void OnOptionSelected (bool selected) {
            Assert.IsFalse (answered);

            if (!answered && selected && HasValidAnswer ()) {
                answered = true;
                SetTogglesEnabled (false);
                StartCoroutine (DelayUtil.Do (0.25f, then));
            }
        }

        public void AskRatingQuestion (WeightQuestion q, Action then) {
            answered = false;
            currQ = q;
            this.then = then;
            toggleGroup.SetAllTogglesOff ();
            SetTogglesEnabled (true);
            optionOneToggle.GetComponentInChildren<Text> ().text = q.OptionOne.ToFriendlyString ();
            optionTwoToggle.GetComponentInChildren<Text> ().text = q.OptionTwo.ToFriendlyString ();
        }

        public WeightAnswer GetWeightAnswer () {
            return new WeightAnswer () {
                WeightId = currQ.Id,
                    Choice = optionOneToggle.isOn ? currQ.OptionOne : currQ.OptionTwo
            };
        }

        public bool HasValidAnswer () {
            return (optionOneToggle.isOn ^ optionTwoToggle.isOn);
        }

        public bool CanClickNext () {
            return HasValidAnswer () && !answered;
        }

        private void SetTogglesEnabled (bool enabled) {
            optionOneToggle.interactable = enabled;
            optionTwoToggle.interactable = enabled;
        }
    }
}