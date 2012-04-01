using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("spell_icons")]
    struct SpellIcon
    {
        [PrimaryKey]
        public uint Id;
        [DBCString(false)]
        public uint Icon;

        public bool FixRow(Locale lang)
        {
            Util.FixIcon(GetType(), Icon);

            return true;
        }
    }
}
