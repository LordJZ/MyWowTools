using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("creature_types")]
    struct CreatureType
    {
        [PrimaryKey]
        public uint Id;
        [DBCString(true)]
        public uint name;
        public uint critter;
    }
}
