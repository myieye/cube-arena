using CubeArena.Assets.MyScripts.Logging.DAL.SQLite;
using CubeArena.Assets.MyScripts.Logging.Survey.Models;

namespace CubeArena.Assets.MyScripts.Logging.DAL.Models.Answers {
    public class WeightAnswer : BaseEntity {
        
        public int _playerRoundId;
        public int _weightId;
        public WeightOption _choice;

        [NotNull]
        public int PlayerRoundId {
            get { return _playerRoundId; }
            set { _playerRoundId = value; }
        }

        [NotNull]
        public int WeightId {
            get { return _weightId; }
            set { _weightId = value; }
        }

        [NotNull]
        public WeightOption Choice {
            get { return _choice; }
            set { _choice = value; }
        }

        public override string ToString () {
            return string.Format (
                "WeightingQuestion: [{0} PlayerRoundId: {1}. WeightingId: {2}. Choice: {3}.]",
                base.ToString (), PlayerRoundId, WeightId, Choice);
        }
    }
}