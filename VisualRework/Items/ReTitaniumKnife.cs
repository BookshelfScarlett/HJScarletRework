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
    public class ReTitaniumKnife : ReVisualItemClass
    {
        public override int ApplyItem => ItemType<TitaniumKnife>();
        public override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualTitaniumKnife = !vp.reVisualTitaniumKnife;
        }
    }
}
