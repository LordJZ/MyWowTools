using System;
using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("char_titles")]
    struct CharTitles
    {
        [PrimaryKey]
        public uint Id;
        private uint unk0;
        [DBCString(true)]
        public uint name;
        [DBCString(true)]
        public uint nameFemale;
        public uint index;
    }
}
