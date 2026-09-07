using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace HJScarletRework.Globals.Configs
{
    public class HJScarletConfigServer : ModConfig
    {
        public static HJScarletConfigServer Instance;
        public override void OnLoaded()
        {
            Instance = this;
        }
        public override ConfigScope Mode => ConfigScope.ServerSide;
        [BackgroundColor(211, 211, 211, 192)]
        [ReloadRequired]
        [DefaultValue(false)]
        public bool CrossModSupport { get; set; }
        [BackgroundColor(211, 211, 211, 192)]
        [ReloadRequired]
        [DefaultValue(false)]
        public bool LostbeltJourneyTestAI { get; set; }
        [BackgroundColor(211, 211, 211, 192)]
        [Range(0, 10f)]
        [DefaultValue(1f)]
        public float ModWeaponDamageMult { get; set; }



    }
}
