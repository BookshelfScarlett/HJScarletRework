using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Rarity.RarityShiny;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.DragonSlayer
{
    [AutoloadEquip(EquipType.Legs)]
    public class DragonSlayerLegs : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Armors;
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 28;
            Item.defense = 30;
            Item.SetUpRarityPrice(ItemRarityID.Purple);
            Item.rare = RarityType<DisasterRarity>();

        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.IsItemName())
            {
                DisasterRarity.DrawRarity2(line);
                return false;
            }
            return true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<GenericDamageClass>() += 10;
            player.moveSpeed += 0.20f;
            player.maxTurrets += 2;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FragmentSolar, 5).
                AddIngredient(ItemID.FragmentVortex, 5).
                AddIngredient(ItemID.FragmentNebula, 5).
                AddIngredient(ItemID.FragmentStardust, 5).
                AddIngredient(ItemID.LunarBar, 5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }

    }
}
