using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.Diver;
using System.Threading.Tasks.Dataflow;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.ExecutorVanillaHead
{
    [AutoloadEquip(EquipType.Head)]
    public class ChlorophyteHeadExecutor :HJScarletArmor
    {
        public override int[] ArmorSlots => [Type,ItemID.ChlorophytePlateMail,ItemID.ChlorophyteGreaves];
        public override bool SetUpArmorSet => true;
        public float Damage = 0.10f;
        public float CritDamage = 0.10f;
        public int Crit = 10;
        public static int BoltDamage = 75;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Damage.ToPercent(),Crit + "%",CritDamage.ToPercent());
        public override void ExSD()
        {
            Item.defense = 30;
            Item.SetUpRarityPrice(ItemRarityID.Lime);
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += Damage;
            player.GetCritChance<ExecutorDamageClass>() += Crit;
            player.HJScarlet().critDamageExecutor += CritDamage;
        }
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            player.setBonus += "\n" + setBonusPath.ToLangValue().ToFormatValue(BoltDamage);
            player.HJScarlet().chlorophyteHeadExecutor = true;
            player.armorEffectDrawShadow = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ChlorophyteBar, 12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

    }
}
