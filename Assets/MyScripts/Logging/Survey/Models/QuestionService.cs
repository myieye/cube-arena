using System.Collections.Generic;
using CubeArena.Assets.MyScripts.Utils;

namespace CubeArena.Assets.MyScripts.Logging.Survey.Models {
    public static class QuestionService {

        private static List<RatingQuestion> ratingQuestions;
        private static List<WeightQuestion> weightQuestions;
        private static List<RatingQuestion> customRatingQuestions;

        static QuestionService () {
            ratingQuestions = new List<RatingQuestion> ();
            
            ratingQuestions.Add (new RatingQuestion (1, "Mental Demand", "How mentally demanding was the task?"));
            ratingQuestions.Add (new RatingQuestion (2, "Physical Demand", "How physically demanding was the task?"));
            /*
            ratingQuestions.Add (new RatingQuestion (3, "Temporal Demand", "How hurried or rushed was the pace of the task?"));
            ratingQuestions.Add (new RatingQuestion (4, "Performance", "How successful were you in accomplishing what you were asked to do?"));
            ratingQuestions.Add (new RatingQuestion (5, "Effort", "How hard did you have to work to accomplish your level of performance?"));
            ratingQuestions.Add (new RatingQuestion (6, "Frustration", "How insecure, discouraged, irritated, stressed, and annoyed were you?"));
            //*/
            
            customRatingQuestions = new List<RatingQuestion> ();
            
            customRatingQuestions.Add (new RatingQuestion (7, "", "I always knew what other players were doing."));
            customRatingQuestions.Add (new RatingQuestion (8, "", "I found it very difficult to work together."));
            /*
            customRatingQuestions.Add (new RatingQuestion (9, "", "I could easily communicate with the other players."));
            customRatingQuestions.Add (new RatingQuestion (10, "", "I found the device was a significant hindrance to my communication."));
            customRatingQuestions.Add (new RatingQuestion (11, "", "I always had a good overview of the play field."));
            customRatingQuestions.Add (new RatingQuestion (12, "", "I would have been much more effective by myself."));
            customRatingQuestions.Add (new RatingQuestion (13, "", "I could reach every area of the play field without dfficulty."));
            customRatingQuestions.Add (new RatingQuestion (14, "", "I collaborated with the other players a lot."));
            customRatingQuestions.Add (new RatingQuestion (15, "", "I was very helpful to the other players."));
            customRatingQuestions.Add (new RatingQuestion (16, "", "The other players were very helpful to me."));
            customRatingQuestions.Add (new RatingQuestion (17, "", "I needed to move a lot in order to accomplish what I wanted to."));
            customRatingQuestions.Add (new RatingQuestion (18, "", "I really enjoyed the game."));
            //*/

            weightQuestions = new List<WeightQuestion> ();
            
            weightQuestions.Add (new WeightQuestion (1, WeightOption.Effort, WeightOption.Performance));
            weightQuestions.Add (new WeightQuestion (2, WeightOption.TemporalDemand, WeightOption.Frustration));
            /*
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
            //*/
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

        public static IList<Question> GetShuffledQuestions () {
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
            return questions;
        }
    }
}