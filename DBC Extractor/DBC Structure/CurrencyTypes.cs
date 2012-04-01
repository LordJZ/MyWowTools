using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("currency_types")]
    [AsEnum("name", "name", "Currency")]
    struct CurrencyTypes
    {
        [PrimaryKey]
        public uint Id;
        public int category;
        [DBCString(true)]
        public uint name;
        [DBCString(false)]
        public uint icon;
        private uint unk1;
        private uint unk2;
        private uint unk3;
        private uint unk4;
        private uint unk5;
        private uint unk6;
        [DBCString(true)]
        public uint description;
    }
}
