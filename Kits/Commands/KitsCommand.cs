using RestoreMonarchy.Kits.Models;
using Rocket.API;
using Rocket.API.Serialisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestoreMonarchy.Kits.Commands
{
    public class KitsCommand : IRocketCommand
    {
        private KitsPlugin pluginInstance => KitsPlugin.Instance;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            List<Permission> permissions = caller.GetPermissions();
            permissions = permissions.FindAll(x => x.Name.StartsWith("kit."));

            List<Kit> kitsWithPermission = pluginInstance.Kits.ToList();
            if (!caller.HasPermission("kits.admin"))
            {
                kitsWithPermission = kitsWithPermission.FindAll(x => permissions.Exists(y => y.Name.Equals($"kit.{x.Name}", System.StringComparison.OrdinalIgnoreCase)));
            }

            if (kitsWithPermission.Count == 0)
            {
                pluginInstance.SendMessageToPlayer(caller, "NoKitsAvailable");
                return;
            }

            StringBuilder sb = new();

            bool hasKitAdmin = caller.HasPermission("kits.admin");
            foreach (Kit kit in kitsWithPermission)
            {
                string kitName = pluginInstance.Translate("KitNameFormat", kit.Name);
                string kitPrice = kit.Price > 0 ? pluginInstance.Translate("KitPriceFormat", kit.Price) : "";
                KitCooldown cooldown = pluginInstance.GetCooldown(caller.Id, kit.Name);
                string kitCooldown = "";
                if (cooldown != null && !hasKitAdmin && cooldown.EndDate > DateTime.Now)
                {
                    TimeSpan timeSpan = cooldown.EndDate - DateTime.Now;
                    string timeLeft = pluginInstance.FormatTimespanShort(timeSpan);
                    kitCooldown = pluginInstance.Translate("KitCooldownFormat", timeLeft);
                }
                sb.Append(" " + kitName + kitPrice + kitCooldown + ",");
            }

            string availableKits = sb.ToString().TrimEnd(',').TrimStart();

            pluginInstance.SendMessageToPlayer(caller, "KitsAvailable", availableKits);
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "kits";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => [];

        public List<string> Permissions => ["kits.admin"];
    }
}
