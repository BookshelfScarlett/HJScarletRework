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
    public class HallowedHeadExecutor : HJScarletArmor
    {
        public override int[] ArmorSlots => [Type,ItemID.HallowedPlateMail,ItemID.HallowedGreaves];
        public override bool SetUpArmorSet => true;
        public float Damage = .10f;
        public float Crti = 10;
        public float CritDamage = .10f;
        public override void ExSD()
        {
            Item.defense = 16;
            Item.SetUpRarityPrice(ItemRarityID.Pink);
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent(),Crti + "%", CritDamage.ToPercent());
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            player.setBonus += "\n" + setBonusPath.ToLangValue();
            //神了瑞德
            player.onHitDodge = true;
            player.armorEffectDrawShadow = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += Damage;
            player.GetCritChance<ExecutorDamageClass>() += Crti;
            player.HJScarlet().critDamageExecutor += CritDamage;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HallowedBar, 12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
