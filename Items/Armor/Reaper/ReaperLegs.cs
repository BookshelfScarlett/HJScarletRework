using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Armor.Diver;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Armor.Reaper
{
    [AutoloadEquip(EquipType.Legs)]
    public class ReaperLegs : HJScarletArmor
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
            Item.defense = 45;
            Item.HJScarlet().CanDrawGhost = true;
            Item.HJScarlet().CanDrawIcon = false;
            Item.SetUpRarityPrice(ItemRarityID.Cyan);
        }
        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += .25f;
            player.aggro += 500;
        }
    }
}
