using System;
using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("item_classes")]
    struct ItemClass
    {
        [PrimaryKey]
        public uint Id;
        private uint unk0;
        private uint unk1; // 1 for weapon, all other 0
        [DBCString(true)]
        public uint name;

        public bool FixRow(Locale lang)
        {
            if (this.name != 0)
            {
                string name = DBC.GetString(GetType(), this.name);
                if (name.IndexOf("OBSOLETE", StringComparison.InvariantCultureIgnoreCase) != -1)
                    return false;
            }

            return true;
        }
    }
}
