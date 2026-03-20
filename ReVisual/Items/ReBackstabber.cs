using ContinentOfJourney.Items;
using HJScarletRework.ReVisual.Class;
using Terraria;

namespace HJScarletRework.ReVisual.Items
{
    public class ReBackstabber : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<Backstabber>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualBackStabber = !vp.reVisualBackStabber;
        }
    }
}
