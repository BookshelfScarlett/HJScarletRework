using ContinentOfJourney.Items;
using HJScarletRework.Particles;
using HJScarletRework.VisualRework.Class;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.VisualRework.Items
{
    public class ReChlorophyteKnife : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<ChlorophyteKnife>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualChlorophyteKnife = !vp.reVisualChlorophyteKnife;
        }
    }
}
