using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("achievement_categories")]
    struct AchievementCategory
    {
        [PrimaryKey]
        public uint Id;
        [Index("category")]
        public int parent;
        [DBCString(true)]
        public uint name;

        public uint order;
    }
}
