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
            permissions = permissions.FindAll(x => x.Name.StartsWith("kit.", StringComparison.OrdinalIgnoreCase));

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

            bool hasKitAdmin = caller.HasPermission("kits.admin");
            List<string> entries = new();
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
                string kitClaims = "";
                if (kit.MaxClaims > 0 && !hasKitAdmin)
                {
                    KitClaim claim = pluginInstance.GetClaim(caller.Id, kit.Name);
                    int count = claim?.Count ?? 0;
                    kitClaims = pluginInstance.Translate("KitClaimsFormat", count, kit.MaxClaims);
                }
                entries.Add(kitName + kitPrice + kitCooldown + kitClaims);
            }

            // Unturned silently truncates a chat message at 2048 bytes on the wire, which
            // cuts mid rich-text tag and makes the whole line render blank. Split the kit
            // list across multiple messages, keeping each well under the limit. Byte counts
            // are measured on the raw [[ ]] markup which is longer than the < > it becomes,
            // so this is a safe over-estimate.
            const int maxMessageBytes = 1800;

            // Only the first message carries the "Your kits:" label, the rest just continue
            // the list as raw text.
            bool isFirstMessage = true;
            void SendChunk(string chunk)
            {
                if (isFirstMessage)
                {
                    pluginInstance.SendMessageToPlayer(caller, "KitsAvailable", chunk);
                }
                else
                {
                    pluginInstance.SendRawMessageToPlayer(caller, chunk);
                }
                isFirstMessage = false;
            }

            StringBuilder sb = new();
            int currentBytes = 0;
            foreach (string entry in entries)
            {
                int entryBytes = Encoding.UTF8.GetByteCount(entry);
                int sepBytes = sb.Length == 0 ? 0 : 2;

                if (sb.Length > 0 && currentBytes + sepBytes + entryBytes > maxMessageBytes)
                {
                    SendChunk(sb.ToString());
                    sb.Clear();
                    currentBytes = 0;
                    sepBytes = 0;
                }

                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(entry);
                currentBytes += sepBytes + entryBytes;
            }

            if (sb.Length > 0)
            {
                SendChunk(sb.ToString());
            }
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "kits";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => [];

        public List<string> Permissions => ["kits.admin"];
    }
}
