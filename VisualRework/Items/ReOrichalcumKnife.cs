using ContinentOfJourney.Items;
using HJScarletRework.VisualRework.Class;
using Terraria;

namespace HJScarletRework.VisualRework.Items
{
    public class ReOrichalcumKnife : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<OrichalcumKnife>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualOrihalcumKnife = !vp.reVisualOrihalcumKnife;
        }
    }
}
