using SDG.Unturned;
using System.Collections.Generic;

namespace RestoreMonarchy.Kits.Helpers
{
    internal static class VehicleHelper
    {
        internal static VehicleAsset GetVehicleByNameOrId(string name)
        {
            List<VehicleAsset> assets = new();
            Assets.find(assets);

            VehicleAsset vehicleAsset = null;
            foreach (VehicleAsset asset in assets)
            {
                if (asset != null &&
                    asset.id.ToString() == name
                    || (asset.FriendlyName != null && asset.FriendlyName.ToLower().Contains(name.ToLower())))
                {
                    vehicleAsset = asset;
                    break;
                }
            }

            return vehicleAsset;
        }
    }
}
