using RestoreMonarchy.Kits.Models.Databases;
using System;
using System.IO;
using System.Xml.Serialization;

namespace RestoreMonarchy.Kits.Database
{
    public class KitCooldownsXmlDatabase
    {
        private KitsPlugin pluginInstance => KitsPlugin.Instance;

        private XmlSerializer xmlSerializer = new(typeof(KitCooldownsDatabase), new XmlRootAttribute(nameof(KitCooldownsDatabase)));
        private string filePath => $"{pluginInstance.Directory}/KitCooldowns.xml";

        public KitCooldownsDatabase Database { get; private set; }

        public void Load()
        {
            if (File.Exists(filePath))
            {
                using StreamReader reader = File.OpenText(filePath);
                Database = (KitCooldownsDatabase)xmlSerializer.Deserialize(reader);
            }
            else
            {
                Database = new KitCooldownsDatabase
                {
                    Cooldowns = []
                };
                Save();
            }
        }

        public void Save()
        {
            Database.Cooldowns.RemoveAll(x => x.EndDate < DateTime.Now);

            using StreamWriter writer = new(filePath);
            xmlSerializer.Serialize(writer, Database);
        }
    }
}
