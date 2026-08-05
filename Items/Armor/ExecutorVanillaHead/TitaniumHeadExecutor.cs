using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.Diver;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.ExecutorVanillaHead
{
    [AutoloadEquip(EquipType.Head)]
    public class TitaniumHeadExecutor : HJScarletArmor
    {
        public override int[] ArmorSlots => [Type, ItemID.TitaniumBreastplate, ItemID.TitaniumLeggings];
        public float Damage = 0.12f;
        public int Crit= 12;
        public float CritDamage = 0.12f;
        public static int ShardDamage = 40;
        public override bool SetUpArmorSet => true;
        public override void ExSD()
        {
            Item.defense = 17;
            Item.SetUpRarityPrice(ItemRarityID.Pink);
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent(),Crit + "%", CritDamage.ToPercent());
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += Damage;
            player.GetCritChance<ExecutorDamageClass>() += Crit;
            player.HJScarlet().critDamageExecutor += CritDamage;
        }
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            player.setBonus += "\n" + setBonusPath.ToLangValue().ToFormatValue(ShardDamage);
            player.armorEffectDrawShadow = true;
            player.HJScarlet().titaniumHeadExecutor = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.TitaniumBar, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
