using System;
using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("area_table")]
    struct AreaTable
    {
        [PrimaryKey]
        public uint Id;
        public uint map;
        /// <summary>
        /// If 0, then it is a zone itself.
        /// </summary>
        private uint parentZone;
        private uint exploreId;
        private uint flags;
        private uint unk0;
        private uint unk1;
        private uint unk2;
        private uint unk3;
        private uint unk4;
        /// <summary>
        /// Can be 0 and -1.
        /// </summary>
        public int level;
        [DBCString(true)]
        public uint name;
        /// <summary>
        /// 2 - Alliance
        /// 4 - Horde
        /// 6 - Both
        /// </summary>
        public uint territory;
        private uint unk6;
        private uint unk7;
        private uint unk8;
        private uint unk9;
        /// <summary>
        /// Something with light?
        /// -500 most of the time, -5000 and 1000 exist.
        /// </summary>
        private float unk10;
        /// <summary>
        /// [0,1]
        /// </summary>
        private float unk11;
        /// <summary>
        /// All 0
        /// </summary>
        private float unk12;

        public bool FixRow(Locale lang)
        {
            if (parentZone != 0)
                return false;

            if (territory != 0)
                return true;

            string name_str = DBC.GetString(GetType(), name);
            if (name_str.IndexOf("reuse", StringComparison.InvariantCultureIgnoreCase) != -1
                || name_str.IndexOf("unused", StringComparison.InvariantCultureIgnoreCase) != -1
                || name_str.IndexOf("test", StringComparison.InvariantCultureIgnoreCase) != -1
                || name_str.IndexOf("do not use", StringComparison.InvariantCultureIgnoreCase) != -1
                || name_str.EndsWith(" Sea")
                || name_str.StartsWith("Transport"))
                return false;

            return true;
        }
    }
}
