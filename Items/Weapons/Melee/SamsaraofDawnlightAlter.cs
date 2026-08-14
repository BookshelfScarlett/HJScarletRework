using ContinentOfJourney.Items;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Projs.Melee;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class SamsaraofDawnlightAlter : HJScarletWeapon
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
        public override EnumDamageClass Category => EnumDamageClass.Melee;
        public override void ExSD()
        {
            Item.CloneDefaults(ItemType<SamsaraOfDawnlight>());
            Item.shoot = ProjectileType<SamsaraofDawnlightProj>();
        }
    }
}
