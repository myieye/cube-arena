using System;
using System.Globalization;
using CubeArena.Assets.MyScripts.Logging.SQLite;

namespace CubeArena.Assets.MyScripts.Logging.Models {
    public class Measurement {

        public Measurement() {
            _created = DateTime.Now.ToString(
                "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

        public int _id;
        public int _playerId;
        public string _created;      
        private int _ui;          
        

        [PrimaryKey, AutoIncrement]
        public int Id {
            get { return _id; }
            set { _id = value; }
        }

        [NotNull]
        public int PlayerId {
            get { return _playerId; }
            set { _playerId = value; }
        }
        
        [NotNull]
        public string Created
        {
            get { return _created;}
            set { _created = value;}
        }
        //[NotNull]
        public int UI
        {
            get { return _ui;}
            set { _ui = value;}
        }

        public override string ToString () {
            return string.Format (
                "Id: {0}. PlayerId: {1}. UI: {2}. Created: {3}.",
                Id, PlayerId, UI, Created);
        }
    }
}