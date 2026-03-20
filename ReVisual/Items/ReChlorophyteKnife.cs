using ContinentOfJourney.Items;
using HJScarletRework.ReVisual.Class;
using Terraria;

namespace HJScarletRework.ReVisual.Items
{
    public class ReChlorophyteKnife : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<ChlorophyteKnife>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualChlorophyteKnife = !vp.reVisualChlorophyteKnife;
        }
    }
}
