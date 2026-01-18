using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances
{
    public class HJScarletGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override void AddRecipes()
        {
            Recipe.Create(ItemID.Spear).
                AddRecipeGroup(HJScarletRecipeGroup.AnyCopperBar, 12).
                AddTile(TileID.Anvils).
                Register();
                
        }
    }
}
