using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Projs.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items
{
    public class Wreach : HJScarletWeapon
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => HJScarletItemProj.Wreach.Path;
        public override void ExSD()
        {
            Item.width = Item.height = 50;
            Item.damage = 20;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ProjectileType<DreamingLightBeam>();
            Item.shootSpeed = 6f;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Vector2 ownerMW = player.LocalMouseWorld();
                Projectile proj = Projectile.NewProjectileDirect(source, position, velocity * 2f, ProjectileType<ClimaticHawstringMinion>(), damage, knockback, player.whoAmI);
            proj.rotation = proj.velocity.ToRotation();
            //添加需要的攻击单位
            return false;
        }
    }
}
