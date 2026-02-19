using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Accessories;
using HJScarletRework.Rarity.RarityShiny;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.DragonSlayer
{
    [AutoloadEquip(EquipType.Legs)]
    public class DragonSlayerLegs : HJScarletItems
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override ItemCategory LocalCategory => ItemCategory.Armor;
        public override string Texture => GetAsset(AssetCategory.Armor);
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 28;
            Item.defense = 30;
            Item.rare = RarityType<DisasterRarity>();
            Item.value = Item.buyPrice(gold: 50);

        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.IsItemName())
            {
                DisasterRarity.DrawRarity(line);
                return false;
            }
            return true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<GenericDamageClass>() += 20f;
            player.maxTurrets += 2;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SquireAltPants).
                AddIngredient(ItemID.HuntressAltPants).
                AddIngredient(ItemID.ApprenticeAltPants).
                AddIngredient(ItemID.MonkAltPants).
                AddIngredient(ItemID.DefenderMedal, 20).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

    }
}
