using RestoreMonarchy.Kits.Database;
using RestoreMonarchy.Kits.Models;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;

namespace RestoreMonarchy.Kits
{
    public class KitsPlugin : RocketPlugin<KitsConfiguration>
    {
        public static KitsPlugin Instance { get; private set; }
        public UnityEngine.Color MessageColor { get; set; }

        public KitsXmlDatabase KitsDatabase { get; private set; }
        public KitCooldownsXmlDatabase KitCooldownsDatabase { get; private set; }

        public List<Kit> Kits => KitsDatabase.Database.Kits;
        public List<KitCooldown> Cooldowns => KitCooldownsDatabase.Database.Cooldowns;

        protected override void Load()
        {
            Instance = this;
            MessageColor = UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, UnityEngine.Color.green);

            KitsDatabase = new KitsXmlDatabase();
            KitsDatabase.Load();
            KitCooldownsDatabase = new KitCooldownsXmlDatabase();
            KitCooldownsDatabase.Load();

            SaveManager.onPostSave += OnPostSave;

            Logger.Log($"{Name} {Assembly.GetName().Version.ToString(3)} has been loaded!", ConsoleColor.Yellow);
        }

        protected override void Unload()
        {
            SaveManager.onPostSave -= OnPostSave;
            KitCooldownsDatabase.Save();

            Logger.Log($"{Name} has been unloaded!", ConsoleColor.Yellow);
        }

        public override TranslationList DefaultTranslations => new()
        {
            { "KitCommandSyntax", "You must specify kit name. Use [[b]]/kits[[/b]] to see the list of available kits." },
            { "KitCommandConsoleSyntax", "You must specify kit name and player you want to receive the kit." },
            { "KitNotFound", "Kit [[b]]{0}[[/b]] not found." },
            { "KitGlobalCooldown", "You have to wait [[b]]{0}[[/b]] before using any kit again." },
            { "KitCooldown", "You have to wait [[b]]{0}[[/b]] before using kit [[b]]{1}[[/b]] again." },
            { "KitNotEnoughMoney", "You can't afford to buy this kit for [[b]]${0}[[/b]] credits." },
            { "KitReceived", "You received kit [[b]]{0}[[/b]]." },
            { "KitReceivedFromSomeone", "You received kit [[b]]{0}[[/b]] from [[b]]{1}[[/b]]." },
            { "KitGiven", "You have given kit [[b]]{0}[[/b]] to [[b]]{1}[[/b]]." },
            { "KitAlreadyExists", "Kit [[b]]{0}[[/b]] already exists." },
            { "CreateKitCommandSyntax", "Usage: /ckit [[name]] [[cooldown]] [price] [experience] [vehicle]" },
            { "CreateKitNameInvalid", "Name must contain no special characters. [[b]]{0}[[/b]] is invalid." },
            { "CreateKitCooldownNotNumber", "Cooldown must be a number. [[b]]{0}[[/b]] is invalid." },
            { "CreateKitPriceNotNumber", "Price must be a number. [[b]]{0}[[/b]] is invalid." },
            { "CreateKitUconomyNotInstalled", "You must install Uconomy plugin to create kits with prices." },
            { "CreateKitExperienceNotNumber", "Experience must be a number. [[b]]{0}[[/b]] is invalid." },
            { "CreateKitVehicleNotFound", "Vehicle [[b]]{0}[[/b]] not found." },
            { "KitCreated", "Created kit [[b]]{0}[[/b]] with [[b]]{1}[[/b]] cooldown and [[b]]{2}[[/b]] items." },
            { "NoKitsAvailable", "You don't have access to any kits." },
            { "KitNameFormat", "[[b]]{0}[[/b]]" },
            { "KitPriceFormat", "[${0}]" },
            { "KitCooldownFormat", "({0})" },
            { "KitsAvailable", "Your kits: {0}" },
            { "DeleteKitCommandSyntax", "You must specify kit name." },
            { "KitDeleted", "Deleted kit [[b]]{0}[[/b]]." },
            { "KitNoPermission", "You don't have permission to use kit [[b]]{0}[[/b]]." },
            { "KitOtherNoPermission", "You don't have permission to give kit to other players." },
            { "PlayerNotFound", "Player {0} not found." },
            { "Day", "1 day" },
            { "Days", "{0} days" },
            { "Hour", "1 hour" },
            { "Hours", "{0} hours" },
            { "Minute", "1 minute" },
            { "Minutes", "{0} minutes" },
            { "Second", "1 second" },
            { "Seconds", "{0} seconds" },
            { "Zero", "a moment" },
            { "DayShort", "{0}d" },
            { "HourShort", "{0}h" },
            { "MinuteShort", "{0}m" },
            { "SecondShort", "{0}s" },
            { "KitAdminBypassPermission", "You have bypassed kit cooldown, because you are admin or have kits.admin permission." }
        };

        public KitCooldown GetCooldown(string steamId, string kitName)
        {
            Cooldowns.RemoveAll(x => x.EndDate < DateTime.Now);

            return Cooldowns.Find(x => x.SteamId == steamId && x.KitName.Equals(kitName, StringComparison.OrdinalIgnoreCase));
        }

        private void OnPostSave()
        {
            KitCooldownsDatabase.Save();
        }

        public string FormatTimespan(TimeSpan span)
        {
            if (span < TimeSpan.FromSeconds(1)) return Translate("Zero");

            List<string> items = new();
            if (span.Days > 0)
                items.Add(span.Days == 1 ? Translate("Day") : Translate("Days", span.Days));
            if (span.Hours > 0)
                items.Add(span.Hours == 1 ? Translate("Hour") : Translate("Hours", span.Hours));
            if (items.Count < 2 && span.Minutes > 0)
                items.Add(span.Minutes == 1 ? Translate("Minute") : Translate("Minutes", span.Minutes));
            if (items.Count < 2 && span.Seconds > 0)
                items.Add(span.Seconds == 1 ? Translate("Second") : Translate("Seconds", span.Seconds));

            return string.Join(" ", items);
        }

        public string FormatTimespanShort(TimeSpan span)
        {
            if (span <= TimeSpan.Zero) return Translate("SecondShort", 0);

            List<string> items = new();
            if (span.Days > 0)
                items.Add(Translate("DayShort", span.Days));
            if (span.Hours > 0)
                items.Add(Translate("HourShort", span.Hours));
            if (items.Count < 2 && span.Minutes > 0)
                items.Add(Translate("MinuteShort", span.Minutes));
            if (items.Count < 2 && span.Seconds > 0)
                items.Add(Translate("SecondShort", span.Seconds));

            return string.Join(" ", items);
        }

        public void SendMessageToPlayer(IRocketPlayer player, string translationKey, params object[] placeholder)
        {
            string msg = Translate(translationKey, placeholder);
            msg = msg.Replace("[[", "<").Replace("]]", ">");
            if (player is ConsolePlayer)
            {
                Logger.Log(msg);
                return;
            }

            UnturnedPlayer unturnedPlayer = (UnturnedPlayer)player;
            ChatManager.serverSendMessage(msg, MessageColor, null, unturnedPlayer.SteamPlayer(), EChatMode.SAY, Configuration.Instance.MessageIconUrl, true);
        }
    }
}