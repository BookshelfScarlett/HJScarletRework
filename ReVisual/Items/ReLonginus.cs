using ContinentOfJourney.Items;
using HJScarletRework.ReVisual.Class;
using Terraria;

namespace HJScarletRework.ReVisual.Items
{
    public class ReLonginus : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<Longinus>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualLonginus = !vp.reVisualLonginus;
        }
    }
}
