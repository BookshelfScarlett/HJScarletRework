using ContinentOfJourney.Items;
using HJScarletRework.ReVisual.Class;
using Terraria;

namespace HJScarletRework.ReVisual.Items
{
    public class ReAdamantiteKnife : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<AdamantiteKnife>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualAdamantiteKnife = !vp.reVisualAdamantiteKnife;
        }
    }
}
