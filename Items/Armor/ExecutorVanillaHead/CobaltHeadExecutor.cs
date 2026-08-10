using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.ExecutorVanillaHead
{
    [AutoloadEquip(EquipType.Head)]
    public class CobaltHeadExecutor : HJScarletArmor
    {
        public override int[] ArmorSlots => [Type, ItemID.CobaltBreastplate, ItemID.CobaltLeggings];
        public override bool SetUpArmorSet => true;
        public float Damage = 0.15f;
        public float CritDamage = 0.15f;
        public override void ExSD()
        {
            Item.defense = 12;
            Item.SetUpRarityPrice(ItemRarityID.LightRed);
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent());
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += Damage;
        }
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            player.setBonus += "\n" + setBonusPath.ToLangValue().ToFormatValue(CritDamage.ToPercent());
            player.HJScarlet().critDamageExecutor += CritDamage;
            player.armorEffectDrawShadow = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.CobaltBar, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
