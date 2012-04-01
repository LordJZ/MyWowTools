using System;
using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("factions")]
    struct Faction
    {
        [PrimaryKey]
        public uint factionID;
        public int reputationListID;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        private uint[] BaseStandingDatas;

        public uint team;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        private uint[] SpilloverDatas;

        [DBCString(true)]
        public uint name;
        [DBCString(true)]
        public uint description;

        /// <summary>
        /// WORKAROUND: reused expansion to fix queries.
        /// </summary>
        public uint side;

        public bool FixRow(Locale lang)
        {
            if (reputationListID < 0)
                return false;

            string name_str = DBC.GetString(GetType(), name);
            if (name_str.IndexOf("reuse", StringComparison.InvariantCultureIgnoreCase) != -1
                || name_str.IndexOf("unused", StringComparison.InvariantCultureIgnoreCase) != -1
                || name_str.IndexOf("test", StringComparison.InvariantCultureIgnoreCase) != -1
                || name_str.IndexOf("do not use", StringComparison.InvariantCultureIgnoreCase) != -1
                || name_str.StartsWith(" "))
                return false;

            side = 0;

            return true;
        }
    }
}
