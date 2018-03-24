using CubeArena.Assets.MyScripts.Logging.SQLite;

namespace CubeArena.Assets.MyScripts.Logging.Models {

    public enum SelectionActionType {
        Select = 1, Deselect = 2, Reselect = 3,
        Miss = 4, Drag = 5, Disallowed = 6
    }

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