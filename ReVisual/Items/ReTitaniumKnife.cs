using ContinentOfJourney.Items;
using HJScarletRework.ReVisual.Class;
using Terraria;

namespace HJScarletRework.ReVisual.Items
{
    public class ReTitaniumKnife : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<TitaniumKnife>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualTitaniumKnife = !vp.reVisualTitaniumKnife;
        }
    }
}
