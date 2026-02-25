using ContinentOfJourney.Items;
using HJScarletRework.VisualRework.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.VisualRework.Items
{
    public class RePalladiumKnife : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<PalladiumKnife>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualPalladiumKnife = !vp.reVisualPalladiumKnife;
        }
    }
}
