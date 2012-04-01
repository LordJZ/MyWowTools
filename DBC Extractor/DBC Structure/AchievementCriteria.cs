using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("achievement_criterias")]
    struct AchievementCriteria
    {
        [PrimaryKey]
        public uint Id;
        [Index("achievement")]
        public uint refAchievement;
        public uint type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        [Array(6)]
        public uint[] value;
        [DBCString(true)]
        public uint name;
        public uint complete_flags;
        public uint group_flags;
        private uint someId;
        public uint timeLimit;
        public uint order;
    }
}
