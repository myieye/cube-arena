using CubeArena.Assets.MyScripts.Logging.DAL.SQLite;

namespace CubeArena.Assets.MyScripts.Logging.DAL.Models.Answers {
    public class RatingAnswer : BaseEntity {
        
        public int _playerRoundId;
        public int _ratingId;
        public int _rating;

        [NotNull]
        public int PlayerRoundId {
            get { return _playerRoundId; }
            set { _playerRoundId = value; }
        }

        [NotNull]
        public int RatingId {
            get { return _ratingId; }
            set { _ratingId = value; }
        }

        [NotNull]
        public int Rating {
            get { return _rating; }
            set { _rating = value; }
        }

        public override string ToString () {
            return string.Format (
                "RatingQuestion: [{0} PlayerRoundId: {1}. RatingId: {2}. Rating: {3}.]",
                base.ToString (), PlayerRoundId, RatingId, Rating);
        }
    }
}