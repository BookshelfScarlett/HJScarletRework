using ContinentOfJourney.Items;
using HJScarletRework.ReVisual.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.ReVisual.Items
{
    public class ReLonginus : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<Longinus>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualLonginus = !vp.reVisualLonginus;
        }
    }
}
