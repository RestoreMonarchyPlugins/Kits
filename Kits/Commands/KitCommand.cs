using RestoreMonarchy.Kits.Models;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestoreMonarchy.Kits.Commands
{
    public class KitCommand : IRocketCommand
    {
        private KitsPlugin pluginInstance => KitsPlugin.Instance;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 1)
            {
                pluginInstance.SendMessageToPlayer(caller, "KitCommandSyntax");
                return;
            }

            string kitName = command[0];
            Kit kit = pluginInstance.Kits.Find(x => x.Name.Equals(kitName, StringComparison.OrdinalIgnoreCase));

            if (kit == null)
            {
                pluginInstance.SendMessageToPlayer(caller, "KitNotFound", kitName);
                return;
            }

            KitCooldown cooldown;
            if (pluginInstance.Configuration.Instance.GlobalCooldown > 0)
            {
                cooldown = pluginInstance.Cooldowns.OrderByDescending(x => x.EndDate).FirstOrDefault();
                if (cooldown != null)
                {
                    TimeSpan timeSpan = cooldown.EndDate.AddSeconds(pluginInstance.Configuration.Instance.GlobalCooldown) - DateTime.Now;
                    if (timeSpan.TotalSeconds > 0)
                    {
                        string timeLeft = pluginInstance.FormatTimespan(timeSpan);
                        pluginInstance.SendMessageToPlayer(caller, "KitGlobalCooldown", timeLeft);
                        return;
                    }
                }
            }

            cooldown = pluginInstance.Cooldowns.Find(x => x.SteamId == caller.Id && x.KitName.Equals(kit.Name, StringComparison.OrdinalIgnoreCase));
            if (cooldown != null && cooldown.EndDate > DateTime.Now) 
            {
                TimeSpan timeSpan = cooldown.EndDate - DateTime.Now;
                string timeLeft = pluginInstance.FormatTimespan(timeSpan);
                pluginInstance.SendMessageToPlayer(caller, "KitCooldown", timeLeft, kit.Name);
                return;
            }

            cooldown = new()
            {
                KitName = kit.Name,
                SteamId = caller.Id,
                EndDate = DateTime.Now.AddSeconds(kit.Cooldown)
            };
            pluginInstance.Cooldowns.Add(cooldown);
            pluginInstance.SendMessageToPlayer(caller, "KitReceived", kit.Name);
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "kit";

        public string Help => "";

        public string Syntax => "<name>";

        public List<string> Aliases => [];

        public List<string> Permissions => [];
    }
}
