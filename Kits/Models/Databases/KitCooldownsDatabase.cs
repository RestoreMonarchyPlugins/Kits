using System.Collections.Generic;
using System.Xml.Serialization;

namespace RestoreMonarchy.Kits.Models.Databases
{
    public class KitCooldownsDatabase
    {
        [XmlArrayItem("Cooldown")]
        public List<KitCooldown> Cooldowns { get; set; }
    }
}
