using HJScarletRework.Projs.General;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Globals.Systems
{
    public static class HJScarletRecipeCallback
    {
        public static void GetGaiaStriker(Recipe recipe, Item item, List<Item> consumedItems, Item destinationStack)
        {
            if (!Main.dedServ)
            {
                Player p = Main.LocalPlayer;
                Vector2 pos = -Vector2.UnitY * 100f + p.MountedCenter;
                Projectile.NewProjectileDirect(p.GetSource_FromThis(), pos, Vector2.UnitY, ProjectileType<GaiaStrikerLootProj>(), 0, 0, p.whoAmI);
            }
        }
    }
}
