using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("quest_infos")]
    struct QuestInfo
    {
        [PrimaryKey]
        public uint Id;
        [DBCString(true)]
        public uint name;
    }
}
