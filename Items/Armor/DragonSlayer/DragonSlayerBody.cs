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
    [AutoloadEquip(EquipType.Body)]
    public class DragonSlayerBody : HJScarletItems
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override ItemCategory LocalCategory => ItemCategory.Armor;
        public override string Texture => GetAsset(AssetCategory.Armor);
        public override void Load()
        {
            //EquipLoader.AddEquipTexture(Mod, )
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 28;
            Item.defense = 35;
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
            player.GetDamage<SummonDamageClass>() += 0.30f;
            player.lifeRegen += 16;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SquireAltShirt).
                AddIngredient(ItemID.HuntressAltShirt).
                AddIngredient(ItemID.ApprenticeAltShirt).
                AddIngredient(ItemID.MonkAltShirt).
                AddIngredient(ItemID.DefenderMedal, 20).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
