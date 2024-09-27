using RestoreMonarchy.Kits.Helpers;
using RestoreMonarchy.Kits.Models;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using SDG.Unturned;
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

            if (caller is ConsolePlayer && command.Length < 2)
            {
                pluginInstance.SendMessageToPlayer(caller, "KitCommandConsoleSyntax");
                return;
            }

            string kitName = command[0];
            Kit kit = pluginInstance.Kits.Find(x => x.Name.Equals(kitName, StringComparison.OrdinalIgnoreCase));

            if (kit == null)
            {
                pluginInstance.SendMessageToPlayer(caller, "KitNotFound", kitName);
                return;
            }

            if (!caller.HasPermission("kits.admin"))
            {
                KitCooldown cooldown;
                if (pluginInstance.Configuration.Instance.GlobalCooldown > 0)
                {
                    cooldown = pluginInstance.Cooldowns.OrderByDescending(x => x.Date).FirstOrDefault();
                    if (cooldown != null)
                    {
                        TimeSpan timeSpan = cooldown.Date.AddSeconds(pluginInstance.Configuration.Instance.GlobalCooldown) - DateTime.Now;
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
                    Date = DateTime.Now,
                    EndDate = DateTime.Now.AddSeconds(kit.Cooldown)
                };
                pluginInstance.Cooldowns.Add(cooldown);
            }

            UnturnedPlayer player;
            if (command.Length > 1)
            {
                if (!caller.HasPermission("kit.other"))
                {
                    pluginInstance.SendMessageToPlayer(caller, "KitOtherNoPermission");
                    return;
                }

                player = UnturnedPlayer.FromName(command[1]);

                if (player == null)
                {
                    pluginInstance.SendMessageToPlayer(caller, "PlayerNotFound", command[1]);
                    return;
                }
            } else
            {
                player = (UnturnedPlayer)caller;
            }

            if (kit.Price > 0 && !caller.HasPermission("kits.admin"))
            {
                if (!UconomyHelper.IsInstalled())
                {
                    Logger.Log("Uconomy is not installed, but kit price is set.");
                    if (caller is not ConsolePlayer)
                    {
                        pluginInstance.SendMessageToPlayer(caller, "You can't buy this kit, because Uconomy plugin is not installed on this server.");
                    }                    
                    return;
                }

                if (!UconomyHelper.TryCharge(caller.Id, kit.Price))
                {
                    pluginInstance.SendMessageToPlayer(caller, "KitNotEnoughMoney", kit.Price);
                    return;
                }
            }
            
            kit.GiveKit(player);
            if (player == caller)
            {
                pluginInstance.SendMessageToPlayer(player, "KitReceived", kit.Name);
            } else
            {
                pluginInstance.SendMessageToPlayer(player, "KitReceivedFromSomeone", kit.Name, caller.DisplayName);
                pluginInstance.SendMessageToPlayer(caller, "KitGiven", kit.Name, player.DisplayName);
            }
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "kit";

        public string Help => "";

        public string Syntax => "<name>";

        public List<string> Aliases => [];

        public List<string> Permissions => ["kits.admin"];
    }
}
