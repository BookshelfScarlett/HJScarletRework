using ContinentOfJourney.Items;
using HJScarletRework.ReVisual.Class;
using Terraria;

namespace HJScarletRework.ReVisual.Items
{
    public class ReDesertScourge : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<DesertScourge>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualDesertScourge = !vp.reVisualDesertScourge;
        }
    }
}
