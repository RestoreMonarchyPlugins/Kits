using RestoreMonarchy.Kits.Models.Databases;
using System.IO;
using System.Xml.Serialization;

namespace RestoreMonarchy.Kits.Database
{
    public class KitsXmlDatabase
    {
        private KitsPlugin pluginInstance => KitsPlugin.Instance;

        private XmlSerializer xmlSerializer = new(typeof(KitsDatabase), new XmlRootAttribute(nameof(KitsDatabase)));
        private string filePath => $"{pluginInstance.Directory}/Kits.xml";

        public KitsDatabase Database { get; private set; }

        public void Load()
        {
            if (File.Exists(filePath))
            {
                using StreamReader reader = File.OpenText(filePath);
                Database = (KitsDatabase)xmlSerializer.Deserialize(reader);
            }
            else
            {
                Database = new KitsDatabase
                {
                    Kits = []
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
