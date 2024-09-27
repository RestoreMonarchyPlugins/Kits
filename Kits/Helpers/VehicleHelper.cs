using SDG.Unturned;
using System.Collections.Generic;

namespace RestoreMonarchy.Kits.Helpers
{
    internal static class VehicleHelper
    {
        internal static Asset GetVehicleByNameOrId(string name)
        {
            VehicleAsset vehicleAsset = null;
            if (ushort.TryParse(name, out ushort id))
            {
                return Assets.find(EAssetType.VEHICLE, id);
            }

            List<VehicleAsset> assets = [];

            Assets.find(assets);
            
            foreach (VehicleAsset asset in assets)
            {
                if (asset != null && (asset.FriendlyName != null && asset.FriendlyName.ToLower().Contains(name.ToLower())))
                {
                    vehicleAsset = asset;
                    break;
                }
            }

            return vehicleAsset;
        }
    }
}
