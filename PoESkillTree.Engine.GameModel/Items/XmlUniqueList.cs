using System.Xml.Serialization;

namespace PoESkillTree.Engine.GameModel.Items
{
    // Contains the classes that allow serialization and deserialization of Uniques.xml

#pragma warning disable 8618 // Initialization is done through deserialization

    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "UniqueList")]
    public class XmlUniqueList
    {
        [XmlElement("Unique")]
        public XmlUnique[] Uniques { get; set; }
    }

    public class XmlUnique
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public int Level { get; set; }

        [XmlAttribute]
        public bool DropDisabled { get; set; }

        public bool ShouldSerializeDropDisabled() => DropDisabled;

        [XmlAttribute]
        public string BaseMetadataId { get; set; }

        public string[] Explicit { get; set; }

        public string[] Properties { get; set; }

    }

#pragma warning restore
}