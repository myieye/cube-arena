using UnityEngine;
using UnityEngine.UI;

namespace CubeArena.Assets.MyScripts.Logging.Survey {
    [RequireComponent (typeof (Surveyer))]
    public class AnswerValidater : MonoBehaviour {

        [SerializeField]
        private Button nextBtn;
        [SerializeField]
        private Button backBtn; 

        private Surveyer surveyer;

        private void Start () {
            surveyer = GetComponent <Surveyer> ();
        }

        private void Update () {
            if (surveyer.SurveyStarted && surveyer.CurrentQuestionAsker != null) {
                nextBtn.interactable = surveyer.CurrentQuestionAsker.CanClickNext ();
                backBtn.interactable = surveyer.CurrentQuestionI > 0;
            } else {
                nextBtn.interactable = false;
                backBtn.interactable = false;
            }
        }
    }
}