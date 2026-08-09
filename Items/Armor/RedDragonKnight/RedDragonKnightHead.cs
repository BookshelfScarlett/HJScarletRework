using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
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
    [AutoloadEquip(EquipType.Head),LegacyName("DragonSlayerHead")]
    public class RedDragonKnightHead : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Armors;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.ScarletRed);
        }
        public override void ExSD()
        {
            Item.defense = 25;
            Item.SetUpRarityPrice(ItemRarityID.Purple);

        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<RedDragonKnightBody>() && legs.type == ItemType<RedDragonKnightLegs>();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            return true;
        }
        public float Damage = .15f;
        public int Crit = 10;
        public float CritDamage = .30f;
        public static int ProgressRegenTime = 5;
        public static float ProgressRegenCount = .05f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent(),Crit + "%");
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += Damage;
            player.GetCritChance<ExecutorDamageClass>() += Crit;
        }
        public override void UpdateArmorSet(Player player)
        {
            string value = Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}.SetBouns").
                ToLangValue().ToFormatValue(CritDamage.ToPercent(), ProgressRegenTime, ProgressRegenCount.ToPercent());
            player.setBonus += "\n" + value;
            player.HJScarlet().redDragonKnight = true;
            player.HJScarlet().critDamageExecutor += CritDamage;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FragmentSolar, 6).
                AddIngredient(ItemID.FragmentVortex, 6).
                AddIngredient(ItemID.FragmentNebula, 6).
                AddIngredient(ItemID.FragmentStardust, 6).
                AddIngredient(ItemID.LunarBar, 6).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
