using HJScarletRework.Items.Armor;
using HJScarletRework.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Instances
{
    public class HJScarletGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            return base.IsArmorSet(head, body, legs);
        }
        public override void AddRecipes()
        {
            Recipe.Create(ItemID.Spear).
                AddRecipeGroup(HJScarletRecipeGroup.AnyCopperBar, 12).
                AddTile(TileID.Anvils).
                Register();

            Recipe.Create(ItemID.SunStone).
                AddIngredient(ItemID.LihzahrdBrick, 25).
                AddIngredient<DisasterEssence>(50).
                DisableDecraft().
                AddTile(TileID.MythrilAnvil).
                Register();

            Recipe.Create(ItemID.DestroyerEmblem).
                AddIngredient(ItemID.Amber, 15).
                AddIngredient<DisasterEssence>(50).
                AddIngredient(ItemID.LihzahrdBrick, 25).
                DisableDecraft().
                AddTile(TileID.MythrilAnvil).
                Register();

        }
    }
}
