using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class StormSpear : HJScarletWeapon
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => HJScarletItemProj.Item_StormSpear.Path;
        public override void ExSD()
        {
            Item.width = 72;
            Item.height = 74;
            Item.damage = 100;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.useTurn = false;
            Item.rare = ItemRarityID.Red;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CobaltBar, 5).
                AddIngredient(ItemID.Cloud, 15).
                AddIngredient(ItemID.SoulofFlight, 10).
                AddIngredient(ItemID.SoulofSight, 10).
                AddIngredient(ItemID.SoulofMight, 10).
                AddIngredient(ItemID.SoulofFright, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
