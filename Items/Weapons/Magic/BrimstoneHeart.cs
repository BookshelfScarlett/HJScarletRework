using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.IDSets;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Methods;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Magic
{
    public class BrimstoneHeart : HJScarletWeaponoutItemClass
    {
        public override EnumDamageClass Category => EnumDamageClass.Magic;
        public override void ExSSD()
        {
            ScarletItemIDSets.IsHeldProjItem[Type] = true;
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.Donator);
        }
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Lime);
            Item.damage = 124;
            Item.useTime = Item.useAnimation = 30;
            Item.shootSpeed = 16f;
            Item.HJScarlet().ItemBelongTo = EnumItemOwner.Donator;
        }
    }
}
