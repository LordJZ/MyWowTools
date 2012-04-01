using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("achievements")]
    struct Achievement
    {
        [PrimaryKey]
        public uint Id;
        public int faction;
        public int map;
        [Index("parent")]
        public uint parent;             // Parent Achievement
        [DBCString(true)]
        public uint name;
        [DBCString(true)]
        public uint description;
        [Index("category")]
        public uint category;
        public uint points;
        public uint order;
        public uint flags;
        public uint icon;
        [DBCString(true)]
        public uint reward;
        public uint count;
        public uint refAchievement;
    }
}
