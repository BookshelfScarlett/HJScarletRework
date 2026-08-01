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
    public class PalladiumHeadExecutor :HJScarletArmor
    {
        public override int[] ArmorSlots => [Type,ItemID.PalladiumBreastplate,ItemID.PalladiumLeggings];
        public override bool SetUpArmorSet => true;
        public float Damage = 0.10f;
        public int LifeRegen = 4;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent(), LifeRegen.ToLifeRegenFormat());
        public override void ExSD()
        {
            Item.defense = 19;
            Item.SetUpRarityPrice(ItemRarityID.LightRed);
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += Damage;
            player.lifeRegen += LifeRegen;
        }
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            player.setBonus += "\n" + setBonusPath.ToLangValue();
            player.palladiumRegen = true;

        }
        public override void AddRecipes()
        {
            if (!HJScarletMethods.HasFuckingCalamity)
            {
                CreateRecipe().
                    AddIngredient(ItemID.PalladiumBar, 10).
                    AddTile(TileID.MythrilAnvil).
                    Register();
            }
            else
            {
                CreateRecipe().
                    AddIngredient(ItemID.PalladiumBar, 10).
                    AddTile(TileID.Anvils).
                    Register();
            }
        }

    }
}
