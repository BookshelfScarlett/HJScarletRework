using ContinentOfJourney.Items;
using HJScarletRework.ReVisual.Class;
using Terraria;

namespace HJScarletRework.ReVisual.Items
{
    public class ReBloodthirst : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<Bloodthirst>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualBloodthirst = !vp.reVisualBloodthirst;
        }
    }
}
