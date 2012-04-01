using System.Runtime.InteropServices;

namespace DbcExtractor
{
    [StructLayout(LayoutKind.Sequential)]
    [TableName("pet_families")]
    struct CreatureFamily
    {
        [PrimaryKey]
        public uint Id;
        private float minScale;
        private uint minScaleLevel;
        private float maxScale;
        private uint maxScaleLevel;
        public uint skill;
        private uint skill_level;
        public uint petFoodMask;
        public int petType;
        private int category;
        [DBCString(true)]
        public uint name;
        [DBCString(false)]
        public uint IconName;

        public bool FixRow(Locale lang)
        {
            Util.FixIcon(GetType(), IconName);

            return true;
        }
    }
}
