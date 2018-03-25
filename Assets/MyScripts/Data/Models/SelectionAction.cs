using CubeArena.Assets.MyScripts.Data.SQLite;
using CubeArena.Assets.MyScripts.Logging.Models;

namespace CubeArena.Assets.MyScripts.Data.Models {

    public class SelectionAction : Measurement {
        public SelectionActionType _type;

        [NotNull]
        public SelectionActionType Type
        {
            get { return _type;}
            set { _type = value;}
        }
        
        public override string ToString () {
            return string.Format (
                "SelectionAction: [{0} Type: {1}.]",
                base.ToString (), Type);
        }
    }
}