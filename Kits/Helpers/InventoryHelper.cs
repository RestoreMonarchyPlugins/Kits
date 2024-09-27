using SDG.Unturned;

namespace RestoreMonarchy.Kits.Helpers
{
    internal static class InventoryHelper
    {
        internal static readonly byte[] EMPTY_BYTE_ARRAY = new byte[0];

        // copied from uEssentials :P
        internal static void ClearPlayerInventory(Player player)
        {
            var playerInv = player.inventory;

            // Remove items
            for (byte page = 0; page < PlayerInventory.PAGES; page++)
            {
                if (page == PlayerInventory.AREA)
                    continue;

                var count = playerInv.getItemCount(page);

                for (byte index = 0; index < count; index++)
                {
                    playerInv.removeItem(page, 0);
                }
            }

            // Remove clothes

            // Remove unequipped cloths
            System.Action removeUnequipped = () => {
                for (byte i = 0; i < playerInv.getItemCount(2); i++)
                {
                    playerInv.removeItem(2, 0);
                }
            };

            // Unequip & remove from inventory
            player.clothing.askWearBackpack(0, 0, EMPTY_BYTE_ARRAY, true);
            removeUnequipped();

            player.clothing.askWearGlasses(0, 0, EMPTY_BYTE_ARRAY, true);
            removeUnequipped();

            player.clothing.askWearHat(0, 0, EMPTY_BYTE_ARRAY, true);
            removeUnequipped();

            player.clothing.askWearPants(0, 0, EMPTY_BYTE_ARRAY, true);
            removeUnequipped();

            player.clothing.askWearMask(0, 0, EMPTY_BYTE_ARRAY, true);
            removeUnequipped();

            player.clothing.askWearShirt(0, 0, EMPTY_BYTE_ARRAY, true);
            removeUnequipped();

            player.clothing.askWearVest(0, 0, EMPTY_BYTE_ARRAY, true);
            removeUnequipped();

            // "Remove "models" of items from player "body""
            player.equipment.sendSlot(0);
            player.equipment.sendSlot(1);
        }

    }
}
