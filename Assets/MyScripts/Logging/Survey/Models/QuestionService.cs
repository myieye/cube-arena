using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils;
using CubeArena.Assets.MyScripts.Utils.Settings;

namespace CubeArena.Assets.MyScripts.Logging.Survey.Models {
    public static class QuestionService {

        private static List<RatingQuestion> ratingQuestions;
        private static List<WeightQuestion> weightQuestions;
        private static List<RatingQuestion> customRatingQuestions;
        private static List<RatingQuestion> oneTimeRatingQuestions;

        static QuestionService () {
            ratingQuestions = new List<RatingQuestion> ();

            ratingQuestions.Add (new RatingQuestion (1, "Mental Demand", "How mentally demanding was the task?"));
            ratingQuestions.Add (new RatingQuestion (2, "Physical Demand", "How physically demanding was the task?"));
            if (Settings.Instance.ForceUserStudySettings) {
                ratingQuestions.Add (new RatingQuestion (3, "Temporal Demand", "How hurried or rushed was the pace of the task?"));
                ratingQuestions.Add (new RatingQuestion (4, "Performance", "How successful were you in accomplishing what you were asked to do?"));
                ratingQuestions.Add (new RatingQuestion (5, "Effort", "How hard did you have to work to accomplish your level of performance?"));
                ratingQuestions.Add (new RatingQuestion (6, "Frustration", "How insecure, discouraged, irritated, stressed, and annoyed were you?"));
            }

            customRatingQuestions = new List<RatingQuestion> ();

            customRatingQuestions.Add (new RatingQuestion (7, "", "I always knew what other players were doing.")); // Collab:overview/FoV
            customRatingQuestions.Add (new RatingQuestion (8, "", "I found it very difficult to work together.")); // Collab:overall
            if (Settings.Instance.ForceUserStudySettings) {
                customRatingQuestions.Add (new RatingQuestion (9, "", "I could easily communicate with the other players.")); // Collab:communication (+)
                customRatingQuestions.Add (new RatingQuestion (10, "", "I found the device was a significant hindrance to my communication.")); // Collab:communication (-)
                customRatingQuestions.Add (new RatingQuestion (11, "", "I could reach every area of the play field without difficulty.")); // Collab:mobility
                customRatingQuestions.Add (new RatingQuestion (12, "", "I collaborated with the other players a lot.")); // Collab:overall
                customRatingQuestions.Add (new RatingQuestion (13, "", "I was very helpful to the other players.")); // Collab:overall
                customRatingQuestions.Add (new RatingQuestion (14, "", "The system seemed very buggy.")); // N/A
                customRatingQuestions.Add (new RatingQuestion (15, "", "I always had a good overview of the play field.")); // Collab:overview/FoV

                //customRatingQuestions.Add (new RatingQuestion (, "", "The other players were very helpful to me."));
                //customRatingQuestions.Add (new RatingQuestion (, "", "I needed to move a lot in order to accomplish what I wanted to."));
                //customRatingQuestions.Add (new RatingQuestion (, "", "I would have been much more effective by myself."));
                //customRatingQuestions.Add (new RatingQuestion (, "", "The system worked flawlessly."));
            }

            oneTimeRatingQuestions = new List<RatingQuestion> ();

            oneTimeRatingQuestions.Add (new RatingQuestion (16, "", "How much experience do you have with the HoloLens?"));
            if (Settings.Instance.ForceUserStudySettings) {
                oneTimeRatingQuestions.Add (new RatingQuestion (17, "", "How much experience do you have with smartphones?"));
                oneTimeRatingQuestions.Add (new RatingQuestion (18, "", "How much experience do you have with augmented reality?"));
            }

            weightQuestions = new List<WeightQuestion> ();

            weightQuestions.Add (new WeightQuestion (1, WeightOption.Effort, WeightOption.Performance));
            weightQuestions.Add (new WeightQuestion (2, WeightOption.TemporalDemand, WeightOption.Frustration));
            if (Settings.Instance.ForceUserStudySettings) {
                weightQuestions.Add (new WeightQuestion (3, WeightOption.TemporalDemand, WeightOption.Effort));
                weightQuestions.Add (new WeightQuestion (4, WeightOption.PhysicalDemand, WeightOption.Frustration));
                weightQuestions.Add (new WeightQuestion (5, WeightOption.Performance, WeightOption.Frustration));
                weightQuestions.Add (new WeightQuestion (6, WeightOption.PhysicalDemand, WeightOption.TemporalDemand));
                weightQuestions.Add (new WeightQuestion (7, WeightOption.PhysicalDemand, WeightOption.Performance));
                weightQuestions.Add (new WeightQuestion (8, WeightOption.TemporalDemand, WeightOption.MentalDemand));
                weightQuestions.Add (new WeightQuestion (9, WeightOption.Frustration, WeightOption.Effort));
                weightQuestions.Add (new WeightQuestion (10, WeightOption.Performance, WeightOption.MentalDemand));
                weightQuestions.Add (new WeightQuestion (11, WeightOption.Performance, WeightOption.TemporalDemand));
                weightQuestions.Add (new WeightQuestion (12, WeightOption.MentalDemand, WeightOption.Effort));
                weightQuestions.Add (new WeightQuestion (13, WeightOption.MentalDemand, WeightOption.PhysicalDemand));
                weightQuestions.Add (new WeightQuestion (14, WeightOption.Effort, WeightOption.PhysicalDemand));
                weightQuestions.Add (new WeightQuestion (15, WeightOption.Frustration, WeightOption.MentalDemand));
            }
        }

        private static IList<RatingQuestion> GetShuffledRatingQuestions () {
            return new List<RatingQuestion> (ratingQuestions).Shuffle ();
        }

        private static IList<RatingQuestion> GetShuffledCustomRatingQuestions () {
            return new List<RatingQuestion> (customRatingQuestions).Shuffle ();
        }

        private static IList<WeightQuestion> GetShuffledWeightQuestions () {
            return new List<WeightQuestion> (weightQuestions).Shuffle ();
        }

        private static IList<RatingQuestion> GetShuffledOneTimeRatingQuestions () {
            return new List<RatingQuestion> (oneTimeRatingQuestions).Shuffle ();
        }

        public static IList<Question> GetShuffledSurveyQuestions (bool withOneTimeQuestions) {
            var questions = new List<Question> ();
            foreach (var q in GetShuffledRatingQuestions ()) {
                questions.Add (q);
            }
            foreach (var q in GetShuffledWeightQuestions ()) {
                questions.Add (q);
            }
            foreach (var q in GetShuffledCustomRatingQuestions ()) {
                questions.Add (q);
            }
            if (withOneTimeQuestions) {
                foreach (var q in GetShuffledOneTimeRatingQuestions ()) {
                    questions.Add (q);
                }
            }
            return questions;
        }
    }
}