using System;
using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("races")]
    struct ChrRaces
    {
        [PrimaryKey]
        public uint Id;                 // 0
        public uint flags;              // 1
        public uint factionTemplate;    // 2
        public uint unk2;               // 3 - icon?
        private uint model_m;           // 4
        private uint model_f;           // 5
        [DBCString(false)]
        private uint abbrev;            // 6 - Hu, Dw, Ta, Gn, ...
        private uint _side;             // 7 - field13 used instead
        private uint unk4;              // 8 - all 7
        private uint unk5;              // 9 - all 15007
        private uint unk6;              // 10 - all 1096
        [DBCString(false)]
        public uint nameSystem;          // 11
        private uint cinematicSequence; // 12
        public uint side;               // 13 - 0 alliance, 1 horde, 2 other
        [DBCString(true)]
        public uint nameMale;
        [DBCString(true)]
        public uint nameFemale;
        [DBCString(true)]
        private uint nameNeutral;
        private uint unk10; // looks like colors
        private uint unk11; // but too high/low
        private uint unk12; // rgb values
        public uint expansion;

        public bool FixRow(Locale lang)
        {
            ++side;
            if (side == 3)
                return false;

            /* Screws the nameMale up
            string systemname = DBC.GetString(GetType(), this.nameSystem);
            if (!String.IsNullOrEmpty(systemname))
                DBC.SetString(GetType(), this.nameSystem, systemname.ToLower());*/

            return true;
        }
    }
}
