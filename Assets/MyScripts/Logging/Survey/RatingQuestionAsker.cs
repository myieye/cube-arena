using CubeArena.Assets.MyScripts.Logging.DAL.Models.Answers;
using CubeArena.Assets.MyScripts.Logging.Survey.Models;
using UnityEngine;
using UnityEngine.UI;

namespace CubeArena.Assets.MyScripts.Logging.Survey {
    public class RatingQuestionAsker : MonoBehaviour, QuestionAsker {

        [SerializeField]
        private Text title;
        [SerializeField]
        private Text question;
        [SerializeField]
        private Slider slider;

        private RatingQuestion currQ;

        public void AskRatingQuestion (RatingQuestion q) {
            currQ = q;
            title.text = q.Title;
            question.text = q.QuestionText;
        }

        public bool HasValidAnswer () {
            return true;
        }

        public RatingAnswer GetRatingAnswer () {
            return new RatingAnswer () {
                RatingId = currQ.Id,
                    Rating = (int) slider.value
            };
        }
    }
}