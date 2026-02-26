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
    public class ReMythrilKnife : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<MythrilKnife>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualMythrilKnife = !vp.reVisualMythrilKnife;
        }
    }
}
