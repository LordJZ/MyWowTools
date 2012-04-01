using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("lfg_dungeons")]
    [AsEnum("systemName", "fullName", "Dungeon")]
    struct LFGDungeons
    {
        [PrimaryKey]
        public uint Id;
        [DBCString(false)]
        public uint name;

        public uint minlevel;
        public uint maxlevel;
        public uint rec_level;
        public uint rec_minlevel;
        public uint rec_maxlevel;
        public int map;
        public uint difficulty;
        public uint unk_25;
        public uint type;
        public int unk;
        [DBCString(false)]
        public uint systemName;
        public uint expansion;
        [DBCString(false)]
        public uint fullName; // reused
        public uint group;
        [DBCString(false)]
        public uint desc;
        private uint unk1;
        private uint unk2;
        private uint unk3;
        private uint unk4;

        static string[] DungeonDifficulties = new string[]
        {
            "Normal", "Heroic", "ERROR 2", "ERROR 3"
        };
        static string[] RaidDifficulties = new string[]
        {
            "Normal", "Normal", "Heroic", "Heroic"
        };

        static Dictionary<Locale, Dictionary<string, List<string>>> s_translations =
            new Dictionary<Locale, Dictionary<string, List<string>>>()
            {
                {
                    Locale.Default,
                    new Dictionary<string, List<string>>()
                    {
                        {
                            "Normal",
                            new List<string>() { " (Normal)", " Dungeon" }
                        },
                        {
                            "Heroic",
                            new List<string>() { " (Heroic)", " Heroic" }
                        },
                        {
                            "25",
                            new List<string>() { " (25)" }
                        },
                        {
                            "10",
                            new List<string>() { " (10)" }
                        },
                    }
                },
                {
                    Locale.ruRU,
                    new Dictionary<string, List<string>>()
                    {
                        {
                            "Normal",
                            new List<string>() { " (норм.)" }
                        },
                        {
                            "Heroic",
                            new List<string>() { " (героич.)" }
                        },
                        {
                            "25",
                            new List<string>() { " (25)" }
                        },
                        {
                            "10",
                            new List<string>() { " (10)" }
                        },
                    }
                },
            };

        public bool FixRow(Locale lang)
        {
            string orig;
            switch (group)
            {
                case 12: // Cataclysm Heroic
                case 5: // Wrath of the Lich King Heroic
                case 3: // Burning Crusade Heroic
                    orig = DBC.GetString(typeof(LFGDungeons), name);
                    if (!s_translations[lang]["Heroic"].Any(v => orig.EndsWith(v)))
                        orig += s_translations[lang]["Heroic"][0];
                    systemName = DBC.CreateString(typeof(LFGDungeons), orig.Enumize());
                    fullName = DBC.CreateString(typeof(LFGDungeons), orig);
                    break;
                case 13: // Cataclysm Normal
                case 4: // Wrath of the Lich King Normal
                case 2: // Burning Crusade Normal
                    orig = DBC.GetString(typeof(LFGDungeons), name);
                    if (!s_translations[lang]["Normal"].Any(v => orig.EndsWith(v)))
                        orig += s_translations[lang]["Normal"][0];
                    systemName = DBC.CreateString(typeof(LFGDungeons), orig.Enumize());
                    fullName = DBC.CreateString(typeof(LFGDungeons), orig);
                    break;
                case 14: // Cataclysm Raid (25)
                case 9: // Wrath of the Lich King Raid (25)
                    orig = DBC.GetString(typeof(LFGDungeons), name);
                    if (!s_translations[lang]["25"].Any(v => orig.EndsWith(v)))
                        orig += s_translations[lang]["25"][0];
                    systemName = DBC.CreateString(typeof(LFGDungeons), orig.Enumize());
                    fullName = DBC.CreateString(typeof(LFGDungeons), orig);
                    break;
                case 15: // Cataclysm Raid (10)
                case 8: // Wrath of the Lich King Raid (10)
                    orig = DBC.GetString(typeof(LFGDungeons), name);
                    if (!s_translations[lang]["10"].Any(v => orig.EndsWith(v)))
                        orig += s_translations[lang]["10"][0];
                    systemName = DBC.CreateString(typeof(LFGDungeons), orig.Enumize());
                    fullName = DBC.CreateString(typeof(LFGDungeons), orig);
                    break;
                case 7: // Burning Crusade Raid
                case 6: // Classic Raid
                case 11: // World Events
                case 0: // other (zones)
                case 1: // Classic Dungeons
                default:
                    systemName = DBC.CreateString(typeof(LFGDungeons), DBC.GetString(typeof(LFGDungeons), name).Enumize());
                    fullName = name;
                    break;
            }

            return true;
        }
    }
}
