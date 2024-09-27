using System;
using System.Xml.Serialization;

namespace RestoreMonarchy.Kits.Models
{
    public class KitCooldown
    {
        [XmlAttribute]
        public string KitName { get; set; }
        [XmlAttribute]
        public string SteamId { get; set; }
        [XmlAttribute]
        public DateTime EndDate { get; set; }
    }
}
