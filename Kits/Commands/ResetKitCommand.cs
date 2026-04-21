using RestoreMonarchy.Kits.Models;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;

namespace RestoreMonarchy.Kits.Commands
{
    public class ResetKitCommand : IRocketCommand
    {
        private KitsPlugin pluginInstance => KitsPlugin.Instance;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 1)
            {
                pluginInstance.SendMessageToPlayer(caller, "ResetKitCommandSyntax");
                return;
            }

            string target = command[0];
            string kitName = command.Length > 1 ? command[1] : null;

            if (kitName != null)
            {
                Kit kit = pluginInstance.Kits.Find(x => x.Name.Equals(kitName, StringComparison.OrdinalIgnoreCase));
                if (kit == null)
                {
                    pluginInstance.SendMessageToPlayer(caller, "KitNotFound", kitName);
                    return;
                }
                kitName = kit.Name;
            }

            int removed;
            if (target.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                if (kitName == null)
                {
                    removed = pluginInstance.Claims.Count;
                    pluginInstance.Claims.Clear();
                }
                else
                {
                    removed = pluginInstance.Claims.RemoveAll(x => x.KitName.Equals(kitName, StringComparison.OrdinalIgnoreCase));
                }

                if (removed == 0)
                {
                    pluginInstance.SendMessageToPlayer(caller, "ResetKitNoClaims", "all players");
                    return;
                }

                pluginInstance.KitClaimsDatabase.Save();
                pluginInstance.SendMessageToPlayer(caller, "ResetKitAll", removed);
                Logger.Log($"{caller.DisplayName} ({caller.Id}) reset {removed} kit claim(s) across all players" + (kitName != null ? $" for kit {kitName}" : ""));
                return;
            }

            string steamId = ResolveSteamId(target);
            removed = kitName == null
                ? pluginInstance.Claims.RemoveAll(x => x.SteamId == steamId)
                : pluginInstance.Claims.RemoveAll(x => x.SteamId == steamId && x.KitName.Equals(kitName, StringComparison.OrdinalIgnoreCase));

            if (removed == 0)
            {
                pluginInstance.SendMessageToPlayer(caller, "ResetKitNoClaims", target);
                return;
            }

            pluginInstance.KitClaimsDatabase.Save();
            pluginInstance.SendMessageToPlayer(caller, "ResetKitPlayer", removed, target);
            Logger.Log($"{caller.DisplayName} ({caller.Id}) reset {removed} kit claim(s) for {target}" + (kitName != null ? $" (kit {kitName})" : ""));
        }

        private static string ResolveSteamId(string input)
        {
            UnturnedPlayer player = UnturnedPlayer.FromName(input);
            return player != null ? player.Id : input;
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "resetkit";

        public string Help => "Reset kit claim history for a player or all players.";

        public string Syntax => "<player|steamId|all> [kitName]";

        public List<string> Aliases => [];

        public List<string> Permissions => ["kits.admin", "kits.reset"];
    }
}
