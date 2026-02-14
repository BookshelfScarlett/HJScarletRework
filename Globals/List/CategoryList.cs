using ContinentOfJourney.Items;
using HJScarletRework.Items.Weapons.Melee;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.List
{
    public class HJScarletCategoryList : ModSystem
    {
        public static List<int> ThrownSpearList = [];
        public static List<int> HJSpearList=[];
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
            HJSpearList =
            [
                ItemType<SweetSweetStab>(),
                ItemType<Candlance>(),
                ItemType<WildPointer>(),
                ItemType<SpearOfDarkness>(),
                ItemType<LightBite>(),
                ItemType<DeepTone>(),
                ItemType<Tonbogiri>(),
                ItemType<FlybackHand>(),
                ItemType<GalvanizedHand>(),
                ItemType<Evolution>(),
                ItemType<Dialectics>(),
                ItemType<SpearOfEscape>()
            ];
        }

        public override void Unload()
        {
            ThrownSpearList = null;
            HJSpearList = null;
        }
    }
}
