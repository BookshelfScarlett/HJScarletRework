using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        [DefaultValue(false)]
        public bool CrossModSupport { get; set; }

        
    }
}
