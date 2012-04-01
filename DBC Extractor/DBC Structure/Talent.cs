using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    [TableName("talents")]
    struct Talent
    {
        [PrimaryKey]
        public uint Id;
        public uint tabId;
        public uint row;
        public uint col;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        [Array(5)]
        public uint[] rank;
        public uint DependsOn;
        private uint DependsOn2;
        private uint DependsOn3;
        public uint DependsOnRank;
        private uint DependsOnRank2;
        private uint DependsOnRank3;
        private uint AddedToSpellBook;
        public uint rankCount;             // Some unknown value reused
        public ulong petFamilyMask; // unk
        public ulong petFamilyMask1; // unk

        public bool FixRow(Locale lang)
        {
            rankCount = 0;
            for (int i = 0; i < rank.Length; ++i)
            {
                if (rank[i] != 0)
                    ++rankCount;
            }

            return true;
        }
    }
}
