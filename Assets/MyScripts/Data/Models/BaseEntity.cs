using System;
using System.Globalization;
using CubeArena.Assets.MyScripts.Data.SQLite;

namespace CubeArena.Assets.MyScripts.Data.Models {
    public abstract class BaseEntity {

        public BaseEntity() {
            _created = DateTime.Now.ToString(
                "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
        }

        public int _id;
        public string _created;      

        [PrimaryKey, AutoIncrement]
        public int Id {
            get { return _id; }
            set { _id = value; }
        }
        
        [NotNull]
        public string Created
        {
            get { return _created;}
            set { _created = value;}
        }

        public override string ToString () {
            return string.Format (
                "Id: {0}. Created: {1}.",
                Id, Created);
        }
    }
}