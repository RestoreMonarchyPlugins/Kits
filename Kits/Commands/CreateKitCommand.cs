using RestoreMonarchy.Kits.Helpers;
using RestoreMonarchy.Kits.Models;
using Rocket.API;
using Rocket.Unturned.Player;
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

            if (kitName.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                pluginInstance.SendMessageToPlayer(caller, "CreateKitNameInvalid", kitName);
                return;
            }

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

            if (price > 0 && !UconomyHelper.IsInstalled())
            {
                pluginInstance.SendMessageToPlayer(caller, "CreateKitUconomyNotInstalled");
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
                VehicleId = vehicleId,
                Items = new()
            };

            UnturnedPlayer player = (UnturnedPlayer)caller;

            PlayerInventory inventory = player.Player.inventory;
            PlayerClothing clothing = player.Player.clothing;

            List<ItemAsset> clothes =
                [
                    clothing.backpackAsset,
                    clothing.glassesAsset,
                    clothing.hatAsset,
                    clothing.maskAsset,
                    clothing.pantsAsset,
                    clothing.shirtAsset,
                    clothing.vestAsset
                ]; 

            foreach (ItemAsset itemAsset in clothes)
            {
                if (itemAsset != null)
                {
                    kit.Items.Add(new KitItem
                    {
                        Id = itemAsset.id,
                        Name = itemAsset.itemName,
                        Amount = 1
                    });
                }
            }

            ItemJar secondaryItem = inventory.getItem(1, 0);
            ItemJar primaryItem = inventory.getItem(0, 0);

            if (secondaryItem != null)
            {
                kit.Items.Add(new KitItem
                {
                    Id = secondaryItem.item.id,
                    Name = secondaryItem.GetAsset<ItemAsset>().itemName,
                    Amount = 1,
                    Metadata = secondaryItem.item.state
                });
            }

            if (primaryItem != null)
            {
                kit.Items.Add(new KitItem
                {
                    Id = primaryItem.item.id,
                    Name = primaryItem.GetAsset<ItemAsset>().itemName,
                    Amount = 1,
                    Metadata = primaryItem.item.state
                });
            }

            for (byte page = 2; page < PlayerInventory.PAGES - 2; page++)
            {
                byte width = inventory.getWidth(page);
                byte height = inventory.getHeight(page);

                List<ItemJar> items = inventory.items[page].items.ToList();

                items = items.OrderBy(x => x.y).ThenBy(x => x.x).ToList();

                foreach (ItemJar item in items)
                {
                    kit.Items.Add(new KitItem
                    {
                        Id = item.item.id,
                        Name = item.GetAsset<ItemAsset>().itemName,
                        Amount = 1,
                        Metadata = item.item.state
                    });
                }
            }

            for (int i = 0; i < kit.Items.Count; i++)
            {
                for (int j = i + 1; j < kit.Items.Count; j++)
                {
                    if (kit.Items[i].Id == kit.Items[j].Id && (kit.Items[i].Metadata?.SequenceEqual(kit.Items[j].Metadata ?? []) ?? kit.Items[j].Metadata == null))
                    {
                        kit.Items[i].Amount++;
                        kit.Items.RemoveAt(j);
                        j--;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            TimeSpan cooldownSpan = TimeSpan.FromSeconds(cooldown);
            string cooldownString = pluginInstance.FormatTimespan(cooldownSpan);

            pluginInstance.Kits.Add(kit);
            pluginInstance.KitsDatabase.Save();
            InventoryHelper.ClearPlayerInventory(player.Player);
            kit.GiveKit(player);
            pluginInstance.SendMessageToPlayer(caller, "KitCreated", kitName, cooldownString, kit.Items.Count);
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "createkit";

        public string Help => "";

        public string Syntax => "<name> <cooldown> [price] [experience] [vehicle]";

        public List<string> Aliases => [ "ckit" ];

        public List<string> Permissions => [];
    }
}
