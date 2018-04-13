using System;
using System.Globalization;
using CubeArena.Assets.MyScripts.Data.SQLite;

namespace CubeArena.Assets.MyScripts.Data.Models {
    public abstract class Measurement : BaseEntity {

        public int _playerRoundId;
        
        [NotNull]
        public int PlayerRoundId
        {
            get { return _playerRoundId;}
            set { _playerRoundId = value;}
        }

        public bool _practiceMode;
        
        [NotNull]
        public bool PracticeMode
        {
            get { return _practiceMode;}
            set { _practiceMode = value;}
        }
        
        
        public override string ToString() {
            return string.Format(
                "{0} PlayerRoundId: {1}.",
                base.ToString(), PlayerRoundId);
        }
    }
}