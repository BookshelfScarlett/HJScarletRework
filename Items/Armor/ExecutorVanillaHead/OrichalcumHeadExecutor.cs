using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.Diver;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.ExecutorVanillaHead
{

    [AutoloadEquip(EquipType.Head)]
    public class OrichalcumHeadExecutor : HJScarletArmor
    {
        public override int[] ArmorSlots => [Type,ItemID.OrichalcumBreastplate,ItemID.OrichalcumLeggings];
        public override bool SetUpArmorSet => true;
        public float CritDamage = .20f;
        public float MoveSpeed = .20f;
        public override void ExSD()
        {
            Item.defense = 15;
            Item.SetUpRarityPrice(ItemRarityID.LightRed);
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritDamage.ToPercent(), MoveSpeed.ToPercent());
        public override void UpdateEquip(Player player)
        {
            player.HJScarlet().critDamageExecutor += CritDamage;
            player.moveSpeed += MoveSpeed;
        }
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            player.setBonus += "\n" + setBonusPath.ToLangValue();
            player.onHitPetal = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.OrichalcumBar, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

    }
}
