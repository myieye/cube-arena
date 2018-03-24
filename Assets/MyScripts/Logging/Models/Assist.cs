using CubeArena.Assets.MyScripts.Logging.SQLite;

namespace CubeArena.Assets.MyScripts.Logging.Models
{
    public enum AssistType { Stacker, Tipper }

    public class Assist : Measurement {

        public int _killId;
        public AssistType _type;
        
        [NotNull]
        public int KillId
        {
            get { return _killId;}
            set { _killId = value;}
        }
        [NotNull]
        public AssistType Type
        {
            get { return _type;}
            set { _type = value;}
        }
        
        public override string ToString() {
            return string.Format (
                "Assist: [{0} KillId: {1}. Type: {2}.]",
                base.ToString (), KillId, Type);
        }
    }
}