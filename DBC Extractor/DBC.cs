using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Security;

namespace DbcExtractor
{
    class DBC
    {
        // ============= STATIC =============

        public static string GetString(Type T, uint id)
        {
            var strs = Strings[T.Name];
            return strs.ContainsKey(id) ? strs[id] : String.Empty;
        }
        public static void SetString(Type T, uint id, string str)
        {
            Strings[T.Name][id] = str;
        }
        public static uint CreateString(Type T, string str)
        {
            //if (!string.IsNullOrEmpty(GetString(T, )))
            Strings[T.Name][++MaxStringEntries[T.Name]] = str;

            return MaxStringEntries[T.Name];
        }

        private static Dictionary<string, Dictionary<uint, string>> Strings = new Dictionary<string, Dictionary<uint, string>>();
        private static Dictionary<string, uint> MaxStringEntries = new Dictionary<string, uint>();

        public static DBC Open<T>(string filename)
        {
            DBC dbc = new DBC(Path.Combine(Program.prefix, filename));
            try
            {
                dbc.m_lang = (Locale)Enum.Parse(typeof(Locale), Program.prefix.Substring(Program.prefix.Length - 4, 4));
            }
            catch
            {
                dbc.m_lang = Locale.Default;
            }
            dbc.Load<T>();
            Console.WriteLine("DBC {0} Loaded.", filename);
            return dbc;
        }

        // ============= INSTANCE =============
        #region DBC Header
        [StructLayout(LayoutKind.Sequential)]
        struct DbcHeader
        {
            private int Signature;
            private int EntryCount;
            private int FieldsCount;
            private int EntrySize;
            private int StringTableSize;

            public int GetStringTableSize() { return StringTableSize; }
            public int GetEntryCount() { return EntryCount; }
            public int GetEntrySize() { return EntrySize; }
            public bool IsDBC() { return Signature == 0x43424457; }
            public long GetDataLength() { return (long)(EntryCount * EntrySize); }
            public long GetStringBlockPos() { return GetDataLength() + (long)Marshal.SizeOf(typeof(DbcHeader)); }
        };
        #endregion

        private List<object> Entries = new List<object>();
        public string Filename { get; private set; }
        public Type EntryType { get; private set; }
        private Locale m_lang;

        public DBC(string fn)
        {
            this.Filename = fn;
        }

        private void Load<T>()
        {
            EntryType = typeof(T);
            string EntryName = EntryType.Name;
            uint maxStringEntry = 0;

            Dictionary<uint, string> strings = new Dictionary<uint, string>();
            using (BinaryReader reader = new BinaryReader(new FileStream(Filename, FileMode.Open, FileAccess.Read), Encoding.UTF8))
            {
                // read dbc header
                DbcHeader header = (DbcHeader)reader.ReadStruct(typeof(DbcHeader));
                int size = Marshal.SizeOf(EntryType);

                if (!header.IsDBC())
                {
                    Console.WriteLine("File {0} is not DBC", Filename);
                    return;
                }

                if (header.GetEntrySize() != size)
                {
                    Console.WriteLine("Error in {0} structure: struct size={1}, in dbc={2}",
                        EntryName, size, header.GetEntrySize());
                    return;
                }

                // read dbc data
                for (int i = 0; i < header.GetEntryCount(); ++i)
                    Entries.Add(reader.ReadStruct(EntryType));

                // read dbc strings
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    var offset = (uint)(reader.BaseStream.Position - header.GetStringBlockPos());
                    var str = reader.ReadCString();
                    strings.Add(offset, str);

                    if (offset > maxStringEntry)
                        maxStringEntry = offset;
                }
            }

            if (Strings.ContainsKey(EntryName))
                Strings.Remove(EntryName);

            if (MaxStringEntries.ContainsKey(EntryName))
                MaxStringEntries.Remove(EntryName);

            MaxStringEntries.Add(EntryName, maxStringEntry);
            Strings.Add(EntryName, strings);

            MethodInfo fixer = EntryType.GetMethod("FixRow");
            if (fixer != null)
            {
                for (int i = 0; i < Entries.Count; )
                {
                    if (!(bool)fixer.Invoke(Entries[i], new object[] { m_lang }))
                        Entries.RemoveAt(i);
                    else
                        ++i;
                }
            }
        }

        public string ToSQL(bool AsLocalization)
        {
            StringBuilder Builder = new StringBuilder();

            String TableName = String.Empty;
            var tablenameArr = (TableNameAttribute[])EntryType.GetCustomAttributes(typeof(TableNameAttribute), true);
            if (tablenameArr.Length != 0)
                TableName = tablenameArr[0].TableName;

            if (String.IsNullOrEmpty(TableName))
            {
                Console.WriteLine("Cannot find [TableName] Attribute for struct {0}", EntryType.Name);
                return String.Empty;
            }

            FieldInfo[] Fields = EntryType.GetFields();

            if (!AsLocalization)
            {
                #region TABLE STRUCTURE

                Builder.AppendFormatLine("DROP TABLE IF EXISTS `{0}_{1}`;", Constants.TablePrefix, TableName);
                Builder.AppendFormatLine("CREATE TABLE `{0}_{1}` (", Constants.TablePrefix, TableName);

                List<String> TableChilds = new List<string>();

                List<String> PrimaryKeys = new List<String>();
                Dictionary<String, List<String>> Indexes = new Dictionary<String, List<String>>();

                #region Building fields
                foreach (FieldInfo Field in Fields)
                {
                    if (Field.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).Length != 0)
                        PrimaryKeys.Add(Field.Name);

                    var Attributes = (IndexAttribute[])Field.GetCustomAttributes(typeof(IndexAttribute), true);
                    foreach (var Attr in Attributes)
                    {
                        if (!Indexes.ContainsKey(Attr.Name))
                            Indexes.Add(Attr.Name, new List<string>());

                        Indexes[Attr.Name].Add(Field.Name);
                    }

                    String FieldType = String.Empty;

                    DBCStringAttribute[] StringAttrs = (DBCStringAttribute[])Field.GetCustomAttributes(typeof(DBCStringAttribute), true);
                    if (StringAttrs.Length != 0)
                    {
                        FieldType = "text";
                        if (StringAttrs[0].Localized)
                        {
                            foreach (Locale loc in Constants.SupportedLocales)
                                TableChilds.Add(String.Format("  `{0}_loc{2}` {1}", Field.Name, FieldType, (byte)loc));
                            continue;
                        }
                    }
                    else
                    {
                        switch (Field.FieldType.Name)
                        {
                            case "Single":
                            case "Single[]":
                                FieldType = "float";
                                break;
                            case "UInt32":
                            case "UInt32[]":
                                FieldType = "int(10) unsigned";
                                break;
                            case "Int32":
                            case "Int32[]":
                                FieldType = "int(10)";
                                break;
                            case "UInt64":
                            case "UInt64[]":
                                FieldType = "bigint(20) unsigned";
                                break;
                            case "Int64":
                            case "Int64[]":
                                FieldType = "bigint(20)";
                                break;
                            default:
                                Console.WriteLine("Unhandled type '{1}' in struct {0}",
                                    EntryType.Name, Field.FieldType.Name);
                                return String.Empty;
                        }
                    }

                    var Attrs = (ArrayAttribute[])Field.GetCustomAttributes(typeof(ArrayAttribute), true);
                    if (Attrs.Length != 0)
                    {
                        int plus = Attrs[0].StartWithZero ? 0 : 1;
                        for (Byte i = 0; i < Attrs[0].Count; ++i)
                            TableChilds.Add(String.Format("  `{0}{2}{3}` {1}", Field.Name, FieldType, Attrs[0].Suffix, i + plus));
                    }
                    else
                        TableChilds.Add(String.Format("  `{0}` {1}", Field.Name, FieldType));
                }
                #endregion

                if (PrimaryKeys.Count != 0)
                    TableChilds.Add(String.Format("  PRIMARY KEY (`{0}`)", String.Join("`, `", PrimaryKeys.ToArray())));

                foreach (var GroupPair in Indexes)
                {
                    TableChilds.Add(String.Format("  INDEX `idx_{0}` (`{1}`)", GroupPair.Key,
                        String.Join("`, `", GroupPair.Value.ToArray())));
                }

                Builder.AppendFormatLine(String.Join("," + Environment.NewLine, TableChilds.ToArray()));
                Builder.AppendFormatLine(") ENGINE=MyISAM DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;");
                Builder.AppendLine();
                Builder.AppendLine();
                #endregion

                Builder.AppendFormatLine("INSERT INTO {0}_{1} VALUES", Constants.TablePrefix, TableName);
                List<String> StringFieldRepr = new List<String>();
                List<String> Lines = new List<String>();
                foreach (Object Entry in Entries)
                {
                    StringFieldRepr.Clear();
                    foreach (FieldInfo Field in Fields)
                    {
                        Object Value = Field.GetValue(Entry);
                        if (Field.GetCustomAttributes(typeof(DBCStringAttribute), true).Length != 0)
                        {
                            IList ids = null;
                            if (Field.FieldType.IsArray)
                            {
                                IList list = ((IList)Value);
                                if (((DBCStringAttribute)Field.GetCustomAttributes(typeof(DBCStringAttribute), true)[0]).Localized)
                                {
                                    ids = new List<uint>();
                                    foreach (Locale loc in Constants.SupportedLocales)
                                        ids.Add(list[(byte)loc]);
                                }
                                else
                                    ids = list;
                            }
                            else
                                ids = new List<uint> { (uint)Value };

                            foreach (uint id in ids)
                                StringFieldRepr.Add(String.Format("'{0}'", GetString(EntryType, id).Escape()));
                        }
                        else if (Field.GetCustomAttributes(typeof(ArrayAttribute), true).Length != 0)
                        {
                            Byte count = ((ArrayAttribute)Field.GetCustomAttributes(typeof(ArrayAttribute), true)[0]).Count;
                            for (Byte i = 0; i < count; ++i)
                                StringFieldRepr.Add(((IFormattable)((IList)Value)[i]).ToString(null, CultureInfo.InvariantCulture));
                        }
                        else
                            StringFieldRepr.Add(((IFormattable)Value).ToString(null, CultureInfo.InvariantCulture));
                    }
                    Lines.Add("(" + String.Join(", ", StringFieldRepr.ToArray()) + ")");
                }
                Builder.Append(String.Join("," + Environment.NewLine, Lines.ToArray()));
                Builder.AppendLine(";");
            }
            else
            {
                List<FieldInfo> PrimaryKeys = new List<FieldInfo>();
                List<FieldInfo> LocalizedStrings = new List<FieldInfo>();

                foreach (FieldInfo Field in Fields)
                {
                    if (Field.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).Length != 0)
                        PrimaryKeys.Add(Field);

                    DBCStringAttribute[] StringAttrs = (DBCStringAttribute[])Field.GetCustomAttributes(typeof(DBCStringAttribute), true);
                    if (StringAttrs.Length != 0)
                    {
                        if (StringAttrs[0].Localized)
                            LocalizedStrings.Add(Field);
                    }
                }

                if (LocalizedStrings.Count == 0)
                    return String.Empty;

                if (PrimaryKeys.Count == 0)
                {
                    Console.WriteLine("Cannot find Primary Key for struct {0}", EntryType.Name);
                    return String.Empty;
                }

                List<String> LocalizedFieldRepr = new List<String>();
                foreach (Object Entry in Entries)
                {
                    LocalizedFieldRepr.Clear();
                    foreach (FieldInfo Info in LocalizedStrings)
                    {
                        uint Ids = (uint)Info.GetValue(Entry);
                        LocalizedFieldRepr.Add(String.Format("{0}_loc{1} = '{2}'",
                            Info.Name, (int)m_lang, GetString(EntryType, Ids).Escape()));
                    }

                    if (LocalizedFieldRepr.Count != 0)
                    {
                        List<String> WhereClauseFieldRepr = new List<String>();
                        foreach (FieldInfo pk in PrimaryKeys)
                        {
                            string key = pk.Name;
                            string value = ((IFormattable)pk.GetValue(Entry)).ToString(null, CultureInfo.InvariantCulture);
                            WhereClauseFieldRepr.Add(String.Format("{0} = {1}", key, value));
                        }
                        Builder.AppendFormatLine("UPDATE {0}_{1} SET {2} WHERE {3};",
                            Constants.TablePrefix, TableName,
                            String.Join(", ", LocalizedFieldRepr.ToArray()),
                            String.Join(" AND ", WhereClauseFieldRepr.ToArray()));
                    }
                }
            }

            Builder.AppendLine();
            Builder.AppendLine();

            return Builder.ToString();
        }

        public string ToEnum()
        {
            var builder = new StringBuilder();

            string translationMember = null;
            string enumMember = null;
            string prefix = null;
            var tablenameArr = (AsEnumAttribute[])EntryType.GetCustomAttributes(typeof(AsEnumAttribute), true);
            if (tablenameArr.Length != 0)
            {
                translationMember = tablenameArr[0].TranslationMember;
                enumMember = tablenameArr[0].EnumMember;
                prefix = tablenameArr[0].Prefix;
            }

            FieldInfo info;

            if (String.IsNullOrEmpty(enumMember) || (info = EntryType.GetField(enumMember)) == null)
            {
                Console.WriteLine("Cannot find (or found invalid) [AsEnum] Attribute for struct {0}", EntryType.Name);
                return String.Empty;
            }

            var pk = EntryType.GetFields().Single(fi => fi.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).Length == 1);
            var translationField = EntryType.GetField(translationMember);

            var names = new List<string>(Entries.Count);
            foreach (Object Entry in Entries)
            {
                var name = GetString(EntryType, (uint)info.GetValue(Entry)).Enumize();
                var value = pk.GetValue(Entry);

                if (names.Contains(name))
                {
                    string newName;
                    int i = 1;
                    do
                    {
                        newName = name + "_" + (++i);
                    } while (names.Contains(newName));
                    name = newName;
                }
                names.Add(name);

                builder.AppendLine("        /// <summary>");
                builder.AppendLine("        /// " + GetString(EntryType, (uint)translationField.GetValue(Entry)));
                builder.AppendLine("        /// </summary>");
                builder.AppendLine("        [LocalizedName(\"" + prefix + value.ToString() + "\")]");
                builder.AppendLine("        " + name + " = " + value.ToString() + ",");
            }

            return builder.ToString();
        }

        public string ToXML()
        {
            var builder = new StringBuilder();

            string translationMember = null;
            string prefix = null;
            var tablenameArr = (AsEnumAttribute[])EntryType.GetCustomAttributes(typeof(AsEnumAttribute), true);
            if (tablenameArr.Length != 0)
            {
                translationMember = tablenameArr[0].TranslationMember;
                prefix = tablenameArr[0].Prefix;
            }

            FieldInfo info;

            if (String.IsNullOrEmpty(translationMember) || (info = EntryType.GetField(translationMember)) == null)
            {
                Console.WriteLine("Cannot find (or found invalid) [AsEnum] Attribute for struct {0}", EntryType.Name);
                return String.Empty;
            }

            var pk = EntryType.GetFields().Single(fi => fi.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).Length == 1);

            foreach (Object Entry in Entries)
            {
                var name = GetString(EntryType, (uint)info.GetValue(Entry));
                var value = pk.GetValue(Entry);

                builder.AppendLine("    <data name=\"" + prefix + value.ToString() + "\" xml:space=\"preserve\">");
                builder.AppendLine("      <value>" + SecurityElement.Escape(name) + "</value>");
                builder.AppendLine("    </data>");
            }

            return builder.ToString();
        }
    }
}
