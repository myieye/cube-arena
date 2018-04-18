using System.Linq;
using CubeArena.Assets.MyScripts.Logging.DAL.SQLite;

namespace CubeArena.Assets.MyScripts.Logging.DAL.Models {
    public class Kill : Measurement {

        public int _level;
        [NotNull]
        public int Level {
            get { return _level; }
            set { _level = value; }
        }

        public override string ToString () {
            return string.Format (
                "Kill: [{0} Level: {1}.]",
                base.ToString (), Level);
        }
    }
}