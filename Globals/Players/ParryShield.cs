using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public void ParryShieldHelper()
        {
            return;
            if (parryShieldType < 0)
                return;
            if (HJScarletList.ParryShieldDictionary.TryGetValue(parryShieldType, out int projID))
            {
                if (!Player.HasProj(projID))
                {
                    Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, projID, 5, 1, Player.whoAmI);
                    ScarletSound(HJScarletSounds.Blunt_Swing, Player.Center, variantType: 2);
                }
            }
        }
    }
}
