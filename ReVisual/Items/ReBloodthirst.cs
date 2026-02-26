using ContinentOfJourney.Items;
using ContinentOfJourney.Tiles.Paintings;
using HJScarletRework.ReVisual.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.ReVisual.Items
{
    public class ReBloodthirst : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<Bloodthirst>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualBloodthirst = !vp.reVisualBloodthirst;
        }
    }
}
