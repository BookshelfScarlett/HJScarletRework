using ContinentOfJourney.Items;
using HJScarletRework.VisualRework.Class;
using Terraria;

namespace HJScarletRework.VisualRework.Items
{
    public class ReHellfire: ReVisualItemClass
    {
        public override int ApplyItem => ItemType<Hellfire>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualHellfire = !vp.reVisualHellfire;
        }
    }
}
