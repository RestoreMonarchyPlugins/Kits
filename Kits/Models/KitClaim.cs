using System;
using System.Xml.Serialization;

namespace RestoreMonarchy.Kits.Models
{
    public class KitClaim
    {
        [XmlAttribute]
        public string KitName { get; set; }
        [XmlAttribute]
        public string SteamId { get; set; }
        [XmlAttribute]
        public int Count { get; set; }
        [XmlAttribute]
        public DateTime LastClaimedAt { get; set; }
    }
}
