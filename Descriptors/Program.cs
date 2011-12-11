using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Descriptors
{
    class Program
    {
        [Flags]
        enum UpdateFieldFlags
        {
            None = 0x00,
            Public = 0x01,
            Private = 0x02,
            OwnerOnly = 0x04,
            Unk1 = 0x08,
            Unk2 = 0x10,
            Unk3 = 0x20,
            GroupOnly = 0x40,
            Unk4 = 0x80,
            Dynamic = 0x100
        }

        enum UpdateFieldType
        {
            None = 0,
            Int = 1,
            TwoShort = 2,
            Float = 3,
            Long = 4,
            Bytes = 5
        }

        static string GlobalParent = "Object";
        struct DerivationPair { public string Parent, Child;  }
        static List<DerivationPair> DerivationRules = new List<DerivationPair>
        {
            new DerivationPair { Child = "Player",      Parent = "Unit" },
            new DerivationPair { Child = "Container",   Parent = "Item" },
        };
        static string GetParent(string Child)
        {
            if (Child == GlobalParent)
                return String.Empty;

            foreach (DerivationPair pair in DerivationRules)
                if (pair.Child == Child)
                    return pair.Parent;

            return GlobalParent;
        }

        static void Main(string[] args)
        {
            var xmlser = new XmlSerializer(typeof(Descriptors));
            Descriptors descs;
            using (var fs = new FileStream("descriptors.xml", FileMode.Open))
                descs = (Descriptors)xmlser.Deserialize(fs);

            foreach (var group in descs.Groups)
            {
                var ds = group.Descriptors;
                for (int i = 0; i < ds.Count; i++)
                {
                    var d = ds[i];

                    if (d.Size > 1)
                    {
                        var size = d.Size;
                        bool addHiParts = false;
                        if (d.Type == 4)
                        {
                            if ((d.Size & 1) != 0)
                                throw new InvalidOperationException();

                            size = d.Size / 2;
                            addHiParts = true;
                            ds.Insert(++i, new Descriptors.Group.Descriptor()
                            {
                                Name = d.Name + "_HIPART",
                                Position = i,
                                Type = d.Type,
                                Flags = d.Flags
                            });
                        }

                        int j = 2;

                        var name = d.Name;
                        int underscoreIdx = name.LastIndexOf('_');
                        int value;
                        if (underscoreIdx != -1 && int.TryParse(name.Substring(underscoreIdx + 1), out value))
                        {
                            j = value + 1;
                            name = name.Substring(0, underscoreIdx);
                        }

                        int lastOne = size + j - 1;
                        for (; j < lastOne; ++j)
                        {
                            ds.Insert(++i, new Descriptors.Group.Descriptor()
                            {
                                Name = name + "_" + j,
                                Position = i,
                                Type = d.Type,
                                Flags = d.Flags
                            });
                            if (addHiParts)
                                ds.Insert(++i, new Descriptors.Group.Descriptor()
                                {
                                    Name = name + "_" + j + "_HIPART",
                                    Position = i,
                                    Type = d.Type,
                                    Flags = d.Flags
                                });
                        }
                    }
                }
            }

            var builder = new StringBuilder(1024 * 400);

            builder
                .AppendLine()
                .AppendLine("// Kamilla")
                .AppendLine("// Auto generated on " + DateTime.Now)
                .AppendLine("namespace Kamilla.WorldOfWarcraft")
                .AppendLine("{");

            var names = new Dictionary<string, string>()
            {
                { "ITEM", "Item" },
                { "GAMEOBJECT", "GameObject" },
                { "DYNAMICOBJECT", "DynamicObject" },
                { "PLAYER", "Player" },
                { "UNIT", "Unit" },
                { "AREATRIGGER", "AreaTrigger" },
                { "OBJECT", "Object" },
                { "CORPSE", "Corpse" },
                { "CONTAINER", "Container" },
            };

            foreach (var group in descs.Groups)
            {
                var lastName = group.Descriptors.Last().Name;

                string groupName;
                if (!names.TryGetValue(lastName.Substring(0, lastName.IndexOf('_')), out groupName))
                    throw new InvalidOperationException("Unknown group: " + lastName);

                builder
                    .AppendLine("    public partial class UpdateFields")
                    .AppendLine("    {")
                    .AppendFormat("        private static UpdateField<{0}UpdateFields>[] _{0}UpdateFields = new UpdateField<{0}UpdateFields>[]", groupName).AppendLine()
                    .AppendLine("        {");

                foreach (var desc in group.Descriptors)
                {
                    builder
                        .AppendFormat("            new UpdateField<{0}UpdateFields>({1}, {2}, {0}UpdateFields.{3}),", groupName, desc.Type, desc.Flags, desc.Name).AppendLine();
                }

                var parent = GetParent(groupName);
                string parentOffset = string.Empty;
                if (!string.IsNullOrEmpty(parent))
                    parentOffset = " - (uint)" + parent + "UpdateFields.End";

                builder
                    .AppendLine("        };")
                    .AppendFormat("        public static UpdateField<{0}UpdateFields> GetUpdateField({0}UpdateFields uf)", groupName).AppendLine()
                    .AppendLine("        {")
                    .AppendFormat("            uint index = (uint)uf{0};", parentOffset).AppendLine()
                    .AppendFormat("            if (index >= (uint){0}UpdateFields.End)", groupName).AppendLine()
                    .AppendFormat("                return UpdateField.CreateUnknown<{0}UpdateFields>(uf);", groupName).AppendLine()
                    .AppendLine()
                    .AppendFormat("            return _{0}UpdateFields[index];", groupName).AppendLine()
                    .AppendLine("        }")
                    .AppendLine("    }")
                    .AppendFormat("    public enum {0}UpdateFields : uint", groupName).AppendLine()
                    .AppendLine("    {");

                if (!string.IsNullOrEmpty(parent))
                    parent += "UpdateFields.End + ";

                int lastPos = -1;
                foreach (var desc in group.Descriptors)
                {
                    builder
                        .AppendFormat("        {0,-62}= {2}0x{1:X4},", desc.Name, desc.Position, parent);

                    if (desc.Size != 0)
                    {
                        builder
                            .AppendFormat(" // Size: {0}, Type: {1}, Flags: {2}", desc.Size, (UpdateFieldType)desc.Type, (UpdateFieldFlags)desc.Flags);
                    }

                    builder.AppendLine();

                    lastPos = desc.Position;
                }

                builder
                    .AppendFormat("        {0,-62}= {2}0x{1:X4}", "End", lastPos + 1, parent).AppendLine()
                    .AppendLine("    }")
                    .AppendLine();
            }

            builder
                .AppendLine("}");

            File.WriteAllText("UpdateFields.Generated.cs", builder.ToString());
        }
    }
}
