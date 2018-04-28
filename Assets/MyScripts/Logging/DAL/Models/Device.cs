using CubeArena.Assets.MyScripts.Logging.DAL.SQLite;
using CubeArena.Assets.MyScripts.PlayConfig.Devices;

namespace CubeArena.Assets.MyScripts.Logging.DAL.Models {
    public class Device : BaseEntity
    {
        public DeviceTypeSpec _type;
        public string _model;

        [NotNull]
        public DeviceTypeSpec Type	
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