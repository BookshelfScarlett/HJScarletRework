using ContinentOfJourney.Items;
using HJScarletRework.ReVisual.Class;
using Terraria;

namespace HJScarletRework.ReVisual.Items
{
    public class ReAccelerationism : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<Accelerationism>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualAccelerationism = !vp.reVisualAccelerationism;
        }
    }
}
