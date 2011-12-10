using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Descriptors
{
    public class Descriptors
    {
        [XmlElement("Group")]
        public Group[] Groups;

        public class Group
        {
            public class Descriptor
            {
                [XmlAttribute]
                public string Name;

                [XmlAttribute]
                public int Position;

                [XmlAttribute]
                public int Size;

                [XmlAttribute]
                public int Type;

                [XmlAttribute]
                public int Flags;
            }

            [XmlElement("Descriptor")]
            public List<Descriptor> Descriptors;
        }
    }
}
