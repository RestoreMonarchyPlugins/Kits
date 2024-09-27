using Rocket.Core.Logging;
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
        public uint Experience { get; set; }
        public bool ShouldSerializeExperience() => Experience > 0;
        [XmlAttribute]
        public ushort VehicleId { get; set; }
        public bool ShouldSerializeVehicleId() => VehicleId > 0;
        [XmlAttribute]
        public decimal Price { get; set; }
        public bool ShouldSerializePrice() => Price > 0;

        [XmlArrayItem("Item")]
        public List<KitItem> Items { get; set; }
        public bool ShouldSerializeItems() => Items != null && Items.Count > 0;

        public void GiveKit(UnturnedPlayer player)
        {
            foreach (KitItem kitItem in Items)
            {
                Item item = new(kitItem.Id, true);
                if (kitItem.Metadata != null && kitItem.Metadata.Length > 0)
                {
                    item.metadata = kitItem.Metadata;
                }

                byte amount = Math.Max((byte)1, kitItem.Amount);
                for (int i = 0; i < amount; i++)
                {
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
                VehicleAsset vehicleAsset = (VehicleAsset)Assets.find(EAssetType.VEHICLE, VehicleId);
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
