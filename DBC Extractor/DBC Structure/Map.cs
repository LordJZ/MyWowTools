using System;
using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("maps")]
    [AsEnum("InternalName", "name", "Map")]
    struct Map
    {
        [PrimaryKey]
        public uint Id;                         // 0
        [DBCString(false)]
        public uint InternalName;               // 1
        public uint type;                       // 2
        public uint flags;                      // 3
        public uint unk;                        // 4
        public uint isBattleground;             // 5
        [DBCString(false)]
        public uint name;                       // 6
        public uint linkedZone;                 // 7
        [DBCString(false)]
        public uint HordeIntro;                 // 8
        [DBCString(false)]
        public uint AllianceIntro;              // 9
        public uint LoadingScreen;              // 10
        public float BattlefieldMapIconScale;   // 11
        public int GhostEntranceMap;            // 12
        public float GhostEntranceX;            // 13
        public float GhostEntranceY;            // 14
        public int TimeOfDayOverride;           // 15
        public uint Expansion;                  // 16
        public uint InstanceResetOffset;        // 17
        public uint MaxPlayers;                 // 18
        public int PhasingMap;                  // 19

        public bool FixRow(Locale lang)
        {
            string name = DBC.GetString(GetType(), this.InternalName);
            if (!String.IsNullOrEmpty(name))
                DBC.SetString(GetType(), this.InternalName, name.Enumize());

            return true;
        }
    }
}
