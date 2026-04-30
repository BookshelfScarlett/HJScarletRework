using System.ComponentModel;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace HJScarletRework.Globals.Configs
{
    public class HJScarletConfigClient : ModConfig, ILocalizedModType
    {
        public static HJScarletConfigClient Instance;
        public override void OnLoaded()
        {
            Instance = this;
        }
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message) => false;
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool SpecialRarity { get; set; }
        [BackgroundColor(192, 54, 64, 192)]
        [Range(50, 50000)]
        [Increment(1)]
        [DefaultValue(10000)]
        public int MaxParticleCounts { get; set; }
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool DrawIcon { get; set; }
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(false)]
        public bool PerformanceMode { get; set; }
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool DrawExecutionCounter { get; set; }


    }
}
