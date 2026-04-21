using System.Collections.Generic;
using System.Xml.Serialization;

namespace RestoreMonarchy.Kits.Models.Databases
{
    public class KitClaimsDatabase
    {
        [XmlArrayItem("Claim")]
        public List<KitClaim> Claims { get; set; }
    }
}
