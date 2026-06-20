using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Requirement;
using HJScarletRework.Projs.Executor;
using Microsoft.Xna.Framework;
using rail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class Frostlight : ExecutorWeaponClass
    {
        public override void ExSSD()
        {
            HJScarletList.FrostRarityHashSet.Add(Type);
        }
        public override void ExSD()
        {
            Item.damage = 60;
            Item.SetUpRarityPrice(ItemRarityID.LightPurple);
            Item.shootSpeed = 21;
            Item.shoot = ProjectileType<FrostlightHeldProj>();
            Item.knockBack = 1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 40;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.UseSound = null;
        }
        public override bool CanUseItem(Player player)
        {
            return (!player.HasProj(Item.shoot) && !player.HasProj<FrostlightHeldProjAlt>());
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.direction = (Main.MouseWorld.X - player.Center.X > 0).ToDirectionInt();
            Vector2 ownerToSky = new Vector2(player.Center.X + 250 * player.direction, player.Center.Y) + new Vector2(0, -500) - player.Center;
            Vector2 skyDir = -(ownerToSky).ToSafeNormalize();
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.originalDamage = damage;
            ((FrostlightHeldProj)proj.ModProjectile).BeginTargetRotation = skyDir.ToRotation();
            ((FrostlightHeldProj)proj.ModProjectile).Flip = 1;
            return false;
        }
    }
}
