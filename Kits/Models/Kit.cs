﻿using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RestoreMonarchy.Kits.Models
{
    public class Kit
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public int Cooldown { get; set; }
        [XmlAttribute]
        public decimal Price { get; set; }
        public bool ShouldSerializePrice() => Price > 0;
        [XmlAttribute]
        public uint Experience { get; set; }
        public bool ShouldSerializeExperience() => Experience > 0;
        [XmlAttribute]
        public ushort VehicleId { get; set; }
        public bool ShouldSerializeVehicleId() => VehicleId > 0;
        [XmlAttribute]
        public string VehicleName { get; set; }
        public bool ShouldSerializeVehicleName() => !string.IsNullOrEmpty(VehicleName);

        [XmlArrayItem("Item")]
        public List<KitItem> Items { get; set; }
        public bool ShouldSerializeItems() => Items != null && Items.Count > 0;

        public void GiveKit(UnturnedPlayer player)
        {
            foreach (KitItem kitItem in Items)
            {
                byte amount = Math.Max((byte)1, kitItem.Amount);
                for (int i = 0; i < amount; i++)
                {
                    Item item = new(kitItem.Id, true);
                    if (kitItem.Metadata != null && kitItem.Metadata.Length > 0)
                    {
                        // copy metadata to prevent reference issues
                        item.metadata = new byte[kitItem.Metadata.Length];
                        Array.Copy(kitItem.Metadata, item.metadata, kitItem.Metadata.Length);

                        // instead of assigning it like this
                        // item.metadata = kitItem.Metadata;
                    }

                    if (!player.Player.inventory.tryAddItem(item, true))
                    {
                        ItemManager.dropItem(item, player.Position, true, true, true);
                    }
                }
            }

            if (Experience > 0)
            {
                player.Experience += Experience;
            }

            if (VehicleId > 0)
            {
                Asset vehicleAsset = Assets.find(EAssetType.VEHICLE, VehicleId);
                if (vehicleAsset != null)
                {
                    player.GiveVehicle(vehicleAsset.id);
                }
                else
                {
                    Logger.Log($"Vehicle with id {VehicleId} was not found");
                }
            }
        }
    }
}
