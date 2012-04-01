using System;
using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("classes")]
    struct ChrClasses
    {
        [PrimaryKey]
        public uint Id;
        public uint flags; // 0x1 hunter, rogue, deathknight, shaman; 0x8 deathknight
        public uint powerType;
        private uint unk0; // 1 for all, 126 for warlocks
        [DBCString(true)]
        public uint nameMale;
        [DBCString(true)]
        public uint nameFemale;
        [DBCString(true)]
        private uint nameNeutral;
        [DBCString(false)]
        public uint nameSystem;
        public uint spellFamily;
        private uint flags2; // unk values
        private uint cinematicSequence;
        public uint expansion;

        public bool FixRow(Locale lang)
        {
            string systemname = DBC.GetString(GetType(), this.nameSystem);
            if (!String.IsNullOrEmpty(systemname))
                DBC.SetString(GetType(), this.nameSystem, systemname.ToLower());

            return true;
        }
    }
}
