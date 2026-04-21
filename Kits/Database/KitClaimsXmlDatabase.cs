using RestoreMonarchy.Kits.Models.Databases;
using System.IO;
using System.Xml.Serialization;

namespace RestoreMonarchy.Kits.Database
{
    public class KitClaimsXmlDatabase
    {
        private KitsPlugin pluginInstance => KitsPlugin.Instance;

        private XmlSerializer xmlSerializer = new(typeof(KitClaimsDatabase), new XmlRootAttribute(nameof(KitClaimsDatabase)));
        private string filePath => $"{pluginInstance.Directory}/KitClaims.xml";

        public KitClaimsDatabase Database { get; private set; }

        public void Load()
        {
            if (File.Exists(filePath))
            {
                using StreamReader reader = File.OpenText(filePath);
                Database = (KitClaimsDatabase)xmlSerializer.Deserialize(reader);
            }
            else
            {
                Database = new KitClaimsDatabase
                {
                    Claims = []
                };
                Save();
            }
        }

        public void Save()
        {
            using StreamWriter writer = new(filePath);
            xmlSerializer.Serialize(writer, Database);
        }
    }
}
