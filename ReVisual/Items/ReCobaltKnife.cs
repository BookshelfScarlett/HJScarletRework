using ContinentOfJourney.Items;
using HJScarletRework.ReVisual.Class;
using Terraria;

namespace HJScarletRework.ReVisual.Items
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
