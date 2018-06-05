namespace CubeArena.Assets.MyScripts.Logging.Survey.Models {
    public class RatingQuestion : Question {

        public RatingQuestion (int id, string title, string questionText) {
            this.Id = id;
            this.Title = title;
            this.QuestionText = questionText;
        }

        public int Id { get; private set; }
        public string Title { get; private set; }
        public string QuestionText { get; private set; }

    }
}