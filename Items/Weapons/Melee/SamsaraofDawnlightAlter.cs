using ContinentOfJourney.Backgrounds;
using ContinentOfJourney.Items;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Projs.Melee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class SamsaraofDawnlightAlter : HJScarletWeapon
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override void ExSD()
        {
            Item.CloneDefaults(ItemType<SamsaraOfDawnlight>());
            Item.shoot = ProjectileType<SamsaraofDawnlightProj>();
        }
    }
}
