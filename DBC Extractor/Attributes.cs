using System;

namespace DbcExtractor
{
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false)]
    public class TableNameAttribute : Attribute
    {
        public String TableName { get; private set; }

        /// <summary>
        /// Defines the SQL table name for this struct.
        /// </summary>
        /// <param name="TableName">The table name — an UNPREFIXED string.</param>
        public TableNameAttribute(String TableName)
        {
            this.TableName = TableName;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PrimaryKeyAttribute : Attribute
    {
        /// <summary>
        /// Defines that this field is a primary key.
        /// </summary>
        public PrimaryKeyAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class IndexAttribute : Attribute
    {
        public string Name { get; private set; }

        /// <summary>
        /// Defines that this field is an index.
        /// </summary>
        /// <param name="id">The group of indexes.</param>
        public IndexAttribute(String Name)
        {
            this.Name = Name;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DBCStringAttribute : Attribute
    {
        public bool Localized { get; private set; }

        /// <summary>
        /// Defines that this field is a string so that string will be extracted from DBC file instead of raw integer value.
        /// </summary>
        /// <param name="Localized">If the string is localized or not.</param>
        public DBCStringAttribute(bool Localized)
        {
            this.Localized = Localized;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ArrayAttribute : Attribute
    {
        public Byte Count { get; private set; }
        public String Suffix { get; set; }
        public bool StartWithZero { get; set; }

        /// <summary>
        /// Defines the SQL table name for this struct.
        /// </summary>
        /// <param name="TableName">The table name — an UNPREFIXED string.</param>
        public ArrayAttribute(Byte Count)
        {
            this.Count = Count;
        }
    }

    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false)]
    public class AsEnumAttribute : Attribute
    {
        public string EnumMember;
        public string TranslationMember;
        public string Prefix;

        public AsEnumAttribute(string enumMember, string translationMember, string prefix)
        {
            this.EnumMember = enumMember;
            this.TranslationMember = translationMember;
            this.Prefix = prefix;
        }
    }
}
