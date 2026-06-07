using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.Diver;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Reaper
{

    [AutoloadEquip(EquipType.Head)]
    public class ReaperHead : HJScarletArmor
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
        public override void SetStaticDefaults()
        {
            HJScarletList.ScarletRarityHashSet.Add(Type);
        }

        public override int[] ArmorSlots => [Type,ItemType<ReaperBody>(),ItemType<ReaperLegs>()];
        public override bool SetUpArmorSet => true;
        public override void ExSD()
        {
            Item.defense = 50;
            Item.HJScarlet().CanDrawGhost = true;
            Item.HJScarlet().CanDrawIcon = false;
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
        }
        public override void UpdateArmorSetBetter(Player player, string setBonusPath)
        {
            player.setBonus += "\n" + setBonusPath.ToLangValue();
            player.lifeRegen += 4;
            player.HJScarlet().diverArmor = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<ExecutorDamageClass>() += .25f;
            player.GetCritChance<ExecutorDamageClass>() += 25;
            player.aggro += 500;
        }
    }
}
