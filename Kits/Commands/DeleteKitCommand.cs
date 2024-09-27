using RestoreMonarchy.Kits.Models;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.Kits.Commands
{
    public class DeleteKitCommand : IRocketCommand
    {
        private KitsPlugin pluginInstance => KitsPlugin.Instance;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 1)
            {
                pluginInstance.SendMessageToPlayer(caller, "DeleteKitCommandSyntax");
                return;
            }

            string kitName = command[0];
            Kit kit = pluginInstance.Kits.Find(x => x.Name.Equals(kitName, StringComparison.OrdinalIgnoreCase));

            if (kit == null)
            {
                pluginInstance.SendMessageToPlayer(caller, "KitNotFound", kitName);
                return;
            }

            pluginInstance.Kits.Remove(kit);
            pluginInstance.Cooldowns.RemoveAll(x => x.KitName.Equals(kit.Name, StringComparison.OrdinalIgnoreCase));
            pluginInstance.KitsDatabase.Save();
            pluginInstance.KitCooldownsDatabase.Save();
            pluginInstance.SendMessageToPlayer(caller, "KitDeleted", kit.Name);
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "deletekit";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => ["dkit"];

        public List<string> Permissions => ["kits.admin"];
    }
}
