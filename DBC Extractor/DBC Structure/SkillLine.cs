using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("skill_lines")]
    struct SkillLine
    {
        [PrimaryKey]
        public uint Id;
        public uint category;

        [DBCString(true)]
        public uint name;
        [DBCString(true)]
        public uint description;

        public uint icon;
        public uint icon2;

        private uint canLink;
    }
}
