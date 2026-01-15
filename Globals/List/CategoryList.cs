using ContinentOfJourney.Items;
using HJScarletRework.Items.Weapons.Melee;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.List
{
    public class HJScarletCategoryList : ModSystem
    {
        public static List<int> ThrownSpearList = [];
        public override void Load()
        {
            //投矛表单
            foreach (var spear in Mod.GetContent<ThrownSpearClass>())
            {
                ThrownSpearList.Add(spear.Type);
            }
            //而后，再添加旅人归途的一些投矛。如果有的话
            ThrownSpearList.Add(ItemType<DesertScourge>());
            ThrownSpearList.Add(ItemType<Longinus>());

        }
        public override void Unload()
        {
            ThrownSpearList = null;
        }
    }
}
