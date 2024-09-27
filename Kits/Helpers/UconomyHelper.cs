using Rocket.Core;

namespace RestoreMonarchy.Kits.Helpers
{
    internal static class UconomyHelper
    {
        internal static bool IsInstalled()
        {
            return R.Plugins.GetPlugin("Uconomy") != null;
        }

        internal static bool TryCharge(string steamId, decimal amount)
        {
            if (amount == 0)
                return true;

            if (fr34kyn01535.Uconomy.Uconomy.Instance.Database.GetBalance(steamId) < amount)
                return false;

            fr34kyn01535.Uconomy.Uconomy.Instance.Database.IncreaseBalance(steamId, -amount);
            return true;
        }
    }
}
