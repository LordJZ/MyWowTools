using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("spell_schools")]
    struct Resistances
    {
        [PrimaryKey]
        public uint Id;
        /// <summary>
        /// 1 for Physical.
        /// </summary>
        private uint unk0;
        /// <summary>
        /// 0 for Physical, unknown 1424-1428 values for others.
        /// </summary>
        private uint unk1;
        [DBCString(true)]
        public uint name;
    }
}
