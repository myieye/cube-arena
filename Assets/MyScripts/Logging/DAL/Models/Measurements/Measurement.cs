using System;
using System.Globalization;
using CubeArena.Assets.MyScripts.Logging.DAL.SQLite;

namespace CubeArena.Assets.MyScripts.Logging.DAL.Models {
    public abstract class Measurement : BaseEntity {

        public int _playerRoundId;

        [NotNull]
        public int PlayerRoundId {
            get { return _playerRoundId; }
            set { _playerRoundId = value; }
        }

        public bool _practiceMode;

        [NotNull]
        public bool PracticeMode {
            get { return _practiceMode; }
            set { _practiceMode = value; }
        }

        public bool _testPhase;

        [NotNull]
        public bool TestPhase {
            get { return _testPhase; }
            set { _testPhase = value; }
        }

        public override string ToString () {
            return string.Format (
                "{0} PlayerRoundId: {1}. PM: {2}. Test Phase: {3}",
                base.ToString (), PlayerRoundId, PracticeMode, TestPhase);
        }
    }
}