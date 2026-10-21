using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Instances.Items;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.WesternBoy
{
    [AutoloadEquip(EquipType.Body)]
    public class WesternBoyBody : HJScarletArmor
    {
        public float Crit = 5;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Crit + "%");
        public override void ExSD()
        {
            Item.defense = 7;
            Item.SetUpRarityPrice(ItemRarityID.Orange);
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<GenericDamageClass>() += Crit;

        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Silk, 15).
                AddRecipeGroup(HJScarletRecipeGroup.AnyEvilScale, 10).
                AddTile(TileID.Hellforge).
                Register();
        }

    }
}
