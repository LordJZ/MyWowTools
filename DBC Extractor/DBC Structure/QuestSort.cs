using System;
using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("quest_sorts")]
    struct QuestSort
    {
        [PrimaryKey]
        public uint Id;
        [DBCString(true)]
        public uint name;

        public bool FixRow(Locale lang)
        {
            string name_str = DBC.GetString(GetType(), name);
            if (name_str.IndexOf("reuse", StringComparison.InvariantCultureIgnoreCase) != -1
                || name_str.IndexOf("unused", StringComparison.InvariantCultureIgnoreCase) != -1
                || name_str.IndexOf("test", StringComparison.InvariantCultureIgnoreCase) != -1
                || name_str.IndexOf("do not use", StringComparison.InvariantCultureIgnoreCase) != -1)
                return false;

            return true;
        }
    }
}
