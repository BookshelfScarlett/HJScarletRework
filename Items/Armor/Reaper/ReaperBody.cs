using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.Diver;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Reaper
{
    [AutoloadEquip(EquipType.Body)]
    public class ReaperBody : HJScarletArmor
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }

        public override void SetStaticDefaults()
        {
            HJScarletList.ScarletRarityHashSet.Add(Type);
        }

        public override void ExSD()
        {
            Item.defense = 60;
            Item.HJScarlet().CanDrawGhost = true;
            Item.HJScarlet().CanDrawIcon = false;
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
        }
        public override void UpdateEquip(Player player)
        {
            player.HJScarlet().critDamageExecutor += 0.15f;
            player.aggro += 500;
        }
    }
}
