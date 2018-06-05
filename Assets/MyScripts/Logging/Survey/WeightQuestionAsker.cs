using CubeArena.Assets.MyScripts.Logging.DAL.Models.Answers;
using CubeArena.Assets.MyScripts.Logging.Survey.Models;
using UnityEngine;
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

        public void AskRatingQuestion (WeightQuestion q) {
            currQ = q;

            toggleGroup.SetAllTogglesOff ();
            optionOneToggle.GetComponentInChildren<Text> ().text = q.OptionOne.ToString ();
            optionTwoToggle.GetComponentInChildren<Text> ().text = q.OptionTwo.ToString ();
        }

        public WeightAnswer GetWeightAnswer () {
            return new WeightAnswer () {
                WeightId = currQ.Id,
                    Choice = optionOneToggle.isOn ? currQ.OptionOne : currQ.OptionTwo
            };
        }

        public bool HasValidAnswer () {
            return optionOneToggle.isOn ^ optionTwoToggle.isOn;
        }
    }
}