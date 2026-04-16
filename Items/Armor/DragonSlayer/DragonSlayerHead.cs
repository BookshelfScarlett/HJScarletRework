using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Rarity.RarityShiny;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.DragonSlayer
{
    [AutoloadEquip(EquipType.Head)]
    public class DragonSlayerHead : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Armors;
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 28;
            Item.defense = 25;
            Item.SetUpRarityPrice(ItemRarityID.Purple);
            Item.rare = RarityType<DisasterRarity>();

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
                DisasterRarity.DrawRarity2(line);
                return false;
            }
            return true;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += 0.15f;
            player.GetCritChance<ExecutorDamageClass>() += 10;
        }
        public override void UpdateArmorSet(Player player)
        {
            string value = Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}.SetBouns").ToLangValue();
            player.HJScarlet().redDragonKnight = true;
            player.setBonus += "\n" + value;
            player.GetAttackSpeed<MeleeDamageClass>() += 0.25f;
            player.maxMinions = 3;
            player.maxTurrets = 3;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FragmentSolar, 10).
                AddIngredient(ItemID.FragmentVortex, 10).
                AddIngredient(ItemID.FragmentNebula, 10).
                AddIngredient(ItemID.FragmentStardust, 10).
                AddIngredient(ItemID.LunarBar, 10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
