using CubeArena.Assets.MyScripts.Logging.DAL.SQLite;
using UnityEngine;

namespace CubeArena.Assets.MyScripts.Logging.DAL.Models {
    public class Device : BaseEntity
    {
        public DeviceType _type;
        public string _model;

        [NotNull]
        public DeviceType Type	
        {
            get { return _type;}
            set { _type = value;}
        }

        [NotNull]
        public string Model	
        {
            get { return _model;}
            set { _model = value;}
        }
    }
}