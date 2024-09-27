using Rocket.API;

namespace RestoreMonarchy.Kits
{
    public class KitsConfiguration : IRocketPluginConfiguration
    {
        public string MessageColor { get; set; }
        public string MessageIconUrl { get; set; }
        public int GlobalCooldown { get; set; }

        public void LoadDefaults()
        {
            MessageColor = "yellow";
            MessageIconUrl = "https://i.imgur.com/ceyPI5h.png";
            GlobalCooldown = 30;
        }
    }
}
