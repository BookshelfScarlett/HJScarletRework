using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Monk
{
    [AutoloadEquip(EquipType.Head)]
    public class MonkHead : HJScarletArmor
    {
        public override int[] ArmorSlots => [Type,ItemType<MonkBody>(),ItemType<MonkLegs>()];
        public override bool SetUpArmorSet => true;
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Yellow);
            Item.defense = 8;
        }
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            player.setBonus += "\n" + setBonusPath.ToLangValue();
            player.HJScarlet().monkExecutor = true;
            player.maxTurrets += 2;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += 0.30f;
        }
    }
}
