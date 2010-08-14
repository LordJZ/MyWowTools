﻿using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("glyphproperties")]
    struct GlyphProperties
    {
        [PrimaryKey]
        public uint Id;
        public uint spellid;
        public uint typeflags;
        public uint iconid;

        public bool FixRow()
        {
            if (spellid == 0)
                return false;

            return true;
        }
    }
}
