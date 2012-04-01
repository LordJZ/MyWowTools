using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("item_sets")]
    struct ItemSet
    {
        [PrimaryKey]
        public uint Id;                 // 0
        [DBCString(true)]
        public uint name;
        private uint _name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        [Array(17)]
        public uint[] item;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        [Array(8)]
        public uint[] spell_bonus;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        [Array(8)]
        public uint[] bonus_items;
        public uint required_skill;
        public uint required_skill_value;
    }
}
