using System.Collections.Generic;
using System.Xml.Serialization;

namespace RestoreMonarchy.Kits.Models
{
    public class Kit
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public int Cooldown { get; set; }
        [XmlAttribute]
        public uint Experience { get; set; }
        public bool ShouldSerializeExperience() => Experience > 0;
        [XmlAttribute]
        public ushort VehicleId { get; set; }
        public bool ShouldSerializeVehicleId() => VehicleId > 0;
        [XmlAttribute]
        public decimal Price { get; set; }
        public bool ShouldSerializePrice() => Price > 0;

        [XmlArrayItem("Item")]
        public List<KitItem> Items { get; set; }
        public bool ShouldSerializeItems() => Items != null && Items.Count > 0;
    }
}
