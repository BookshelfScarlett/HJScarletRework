using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.RedDragonKnight
{
    [AutoloadEquip(EquipType.Body), LegacyName("DragonSlayerBody")]
    public class RedDragonKnightBody : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Armors;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.ScarletRed);
        }
        public override void ExSD()
        {
            Item.defense = 35;
            Item.SetUpRarityPrice(ItemRarityID.Purple);

        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            return true;
        }
        public float Damage = 0.05f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent());
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<GenericDamageClass>() += Damage;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FragmentSolar, 8).
                AddIngredient(ItemID.FragmentVortex, 8).
                AddIngredient(ItemID.FragmentNebula, 8).
                AddIngredient(ItemID.FragmentStardust, 8).
                AddIngredient(ItemID.LunarBar, 8).
                AddTile(TileID.LunarCraftingStation).
                Register();

        }
    }
}
