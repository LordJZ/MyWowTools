using System;
using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("item_subclasses")]
    struct ItemSubClass
    {
        [PrimaryKey]
        public uint ClassId;
        [PrimaryKey]
        public uint SubClassId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        [Array(8)]
        private uint[] unks;
        [DBCString(true)]
        public uint name;
        [DBCString(true)]
        public uint desc;

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
