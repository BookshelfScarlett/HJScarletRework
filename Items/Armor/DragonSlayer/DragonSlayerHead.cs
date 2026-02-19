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
    [AutoloadEquip(EquipType.Head)]
    public class DragonSlayerHead : HJScarletItems
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override ItemCategory LocalCategory => ItemCategory.Armor;
        public override string Texture => GetAsset(AssetCategory.Armor);
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 28;
            Item.defense = 25;
            Item.rare = RarityType<DisasterRarity>();
            Item.value = Item.buyPrice(gold: 50);

        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<DragonSlayerBody>() && legs.type == ItemType<DragonSlayerLegs>();
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
            player.GetDamage<GenericDamageClass>() += 0.25f;
            player.maxTurrets += 2;
        }
        public override void UpdateArmorSet(Player player)
        {
            string value = Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}.SetBonus").ToLangValue();
            player.setBonus += value;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SquireAltHead).
                AddIngredient(ItemID.HuntressAltHead).
                AddIngredient(ItemID.ApprenticeAltHead).
                AddIngredient(ItemID.MonkAltHead).
                AddIngredient(ItemID.DefenderMedal, 20).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
