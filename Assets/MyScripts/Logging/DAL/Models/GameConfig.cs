using CubeArena.Assets.MyScripts.Logging.DAL.SQLite;

namespace CubeArena.Assets.MyScripts.Logging.DAL.Models {
    public class GameConfig : BaseEntity {
        
        [NotNull]
        public int NumberOfPlayers { get; set; }
        [NotNull]
        public int NumberOfRounds { get; set; }
    }
}