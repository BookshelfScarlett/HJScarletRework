using HJScarletRework.Items.Useables;
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
