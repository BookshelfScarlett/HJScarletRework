using ContinentOfJourney.Items;
using HJScarletRework.VisualRework.Class;
using Terraria;

namespace HJScarletRework.VisualRework.Items
{
    public class ReCobaltKnife : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<CobaltKnife>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualCobaltKnife = !vp.reVisualCobaltKnife;
        }
    }
}
