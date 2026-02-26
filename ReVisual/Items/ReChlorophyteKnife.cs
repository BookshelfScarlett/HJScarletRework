using ContinentOfJourney.Items;
using HJScarletRework.Particles;
using HJScarletRework.ReVisual.Class;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.ReVisual.Items
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
