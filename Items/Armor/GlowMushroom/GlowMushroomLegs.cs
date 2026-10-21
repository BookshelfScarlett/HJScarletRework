using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.GlowMushroom
{
    [AutoloadEquip(EquipType.Legs)]
    public class GlowMushroomLegs : HJScarletArmor
    {
        public float MoveSpeed = .05f;
        public override void ExSD()
        {
            Item.defense = 3;
            Item.SetUpRarityPrice(ItemRarityID.Green);
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MoveSpeed.ToPercent());
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += MoveSpeed;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.GlowingMushroom, 10).
                AddTile(TileID.WorkBenches).
                Register();
        }

    }
}
