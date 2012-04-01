using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("pet_foods")]
    struct ItemPetFood
    {
        [PrimaryKey]
        public uint Id;
        [DBCString(true)]
        public uint name;
    }
}
