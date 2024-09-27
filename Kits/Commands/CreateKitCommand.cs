using RestoreMonarchy.Kits.Helpers;
using RestoreMonarchy.Kits.Models;
using Rocket.API;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.Kits.Commands
{
    public class CreateKitCommand : IRocketCommand
    {
        private KitsPlugin pluginInstance => KitsPlugin.Instance;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 2)
            {
                pluginInstance.SendMessageToPlayer(caller, "CreateKitCommandSyntax");
                return;
            }

            string kitName = command[0];
            Kit kit = pluginInstance.Kits.FirstOrDefault(x => x.Name.Equals(kitName, StringComparison.OrdinalIgnoreCase));
            if (kit != null)
            {
                pluginInstance.SendMessageToPlayer(caller, "KitAlreadyExists", kit.Name);
                return;
            }

            if (!int.TryParse(command[1], out int cooldown))
            {
                pluginInstance.SendMessageToPlayer(caller, "CreateKitCooldownNotNumber", command[1]);
                return;
            }

            uint price = 0;
            if (command.Length > 2 && !uint.TryParse(command[2], out price))
            {
                pluginInstance.SendMessageToPlayer(caller, "CreateKitPriceNotNumber", command[2]);
                return;
            }

            uint experience = 0;
            if (command.Length > 3 && !uint.TryParse(command[3], out experience))
            {
                pluginInstance.SendMessageToPlayer(caller, "CreateKitExperienceNotNumber", command[3]);
                return;
            }

            ushort vehicleId = 0;
            if (command.Length > 4)
            {
                VehicleAsset vehicleAsset = VehicleHelper.GetVehicleByNameOrId(command[4]);
                if (vehicleAsset == null)
                {
                    pluginInstance.SendMessageToPlayer(caller, "CreateKitVehicleNotFound", command[4]);
                    return;
                }

                vehicleId = vehicleAsset.id;
            }

            kit = new()
            {
                Name = kitName,
                Cooldown = cooldown,
                Price = price,
                Experience = experience,
                VehicleId = vehicleId
            };

            TimeSpan cooldownSpan = TimeSpan.FromSeconds(cooldown);
            string cooldownString = pluginInstance.FormatTimespan(cooldownSpan);

            pluginInstance.Kits.Add(kit);
            pluginInstance.KitsDatabase.Save();
            pluginInstance.SendMessageToPlayer(caller, "KitCreated", kitName, cooldownString);
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "createkit";

        public string Help => "";

        public string Syntax => "<name> <cooldown> [price] [experience] [vehicle]";

        public List<string> Aliases => [ "ckit" ];

        public List<string> Permissions => [];
    }
}
