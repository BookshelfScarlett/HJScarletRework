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
    [AutoloadEquip(EquipType.Body)]
    public class DragonSlayerBody : HJScarletItemClass
    {
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override string AssetPath => AssetHandler.Armor;
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
