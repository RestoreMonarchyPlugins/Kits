using System.Xml.Serialization;

namespace RestoreMonarchy.Kits.Models
{
    public class KitItem
    {
        [XmlAttribute]
        public ushort Id { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        public bool ShouldSerializeName() => !string.IsNullOrEmpty(Name);
        [XmlAttribute]
        public byte Amount { get; set; }
        public bool ShouldSerializeAmount() => Amount != 1;
        [XmlAttribute]
        public byte[] Metadata { get; set; }
        public bool ShouldSerializeMetadata() => Metadata != null && Metadata.Length > 0;
    }
}
