using ContinentOfJourney.Items;
using HJScarletRework.ReVisual.Class;
using Terraria;

namespace HJScarletRework.ReVisual.Items
{
    public class ReVisualPurplePuffer : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<PurplePuffer>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualPurplePuffer = !vp.reVisualPurplePuffer;
        }
    }
}
