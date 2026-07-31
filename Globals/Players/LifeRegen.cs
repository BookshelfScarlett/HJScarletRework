using HJScarletRework.Items.Useables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public override void UpdateLifeRegen()
        {
            if (fruitofEthernity)
            {
                Player.lifeRegen += FruitofEternity.LifeRegenSpeed;
            }
        }

    }
}
