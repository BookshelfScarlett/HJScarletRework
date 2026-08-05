using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Ranged
{
    public class BlazingSun : HJScarletWeapon
    {
        public override EnumDamageClass Category => EnumDamageClass.Ranged;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.Solar);
        }
        public override void ExSD()
        {
            Item.damage = 60;
            Item.useTime = Item.useAnimation = 20;
            Item.knockBack = 5f;
            Item.SetUpRarityPrice(ItemRarityID.LightRed);
            Item.SetUpNoUseGraphicItem(true, false);
            Item.HJScarlet().NotFinished = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileType<ClimaticHawstringProj>();
            Item.shootSpeed = 12f;
        }
        public override bool CanShoot(Player player)
        {
            return base.CanShoot(player);
        }
        public override void HoldItem(Player player)
        {
            base.HoldItem(player);
        }
    }
}
