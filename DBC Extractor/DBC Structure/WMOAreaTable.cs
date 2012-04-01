using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("wmoareatable")]
    struct WMOAreaTable
    {
        [PrimaryKey]
        public uint Id;
        public uint zone;
        private uint unk0;
        public int unk1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        private uint[] unk2_9;
        [DBCString(true)]
        public uint name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        private uint[] unk3_3;
    }
}
