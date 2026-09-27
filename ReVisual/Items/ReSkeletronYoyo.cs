using ContinentOfJourney.Items;
using HJScarletRework.ReVisual.Class;
using Terraria;

namespace HJScarletRework.ReVisual.Items
{
    public class ReSkeletronYoyo : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<SkeletronYoyo>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reSkeletronYoyo = !vp.reSkeletronYoyo;
        }
    }
}
