using System;
using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("talent_tabs")]
    struct TalentTab
    {
        [PrimaryKey]
        public uint id;
        [DBCString(true)]
        public uint name;
        public uint icon;
        public uint classId;
        public uint petType;
        public uint order;
        [DBCString(false)]
        private uint internalName;
        [DBCString(true)]
        public uint description;
        public uint roles;
        public uint spell1;
        public uint spell2;

        public bool FixRow(Locale lang)
        {
            if (classId != 0)
                classId = (uint)Math.Log(classId, 2) + 1;

            if (petType != 0)
                petType = (uint)Math.Log(petType, 2) + 1;

            return true;
        }
    }
}
