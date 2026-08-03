using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Systems;
using HJScarletRework.Items.Armor.Diver;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.ExecutorVanillaHead
{
    [AutoloadEquip(EquipType.Head)]
    public class AdamantiteHeadExecutor : HJScarletArmor
    {
        public override int[] ArmorSlots => [Type, ItemID.AdamantiteBreastplate, ItemID.AdamantiteLeggings];
        public float Damage = 0.07f;
        public float CritDamage = 0.14f;
        public float Crit = 7;
        public static int StrikeChance = 2;
        public static int ThunderDamage = 100;
        public static int ThunderCount = 8;
        public static int ThunderCritChance = 4;
        public override bool SetUpArmorSet => true;
        public override void ExSD()
        {
            Item.defense = 18;
            Item.SetUpRarityPrice(ItemRarityID.Pink);
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent(), Crit + "%");
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += Damage;
            player.GetCritChance<ExecutorDamageClass>() += Crit;
        }
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            player.setBonus += "\n" + setBonusPath.ToLangValue().ToFormatValue(CritDamage.ToPercent(),StrikeChance,ThunderCount,ThunderDamage,ThunderCritChance);
            player.armorEffectDrawShadow = true;
            player.HJScarlet().adamantiteHeadExecutor = true;
            player.HJScarlet().critDamageExecutor += CritDamage;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.AdamantiteBar, 13).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
