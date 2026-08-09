using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.IDSets;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.Misc
{
    public class TheGreatDipper : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 7;
        public override void ExSSD()
        {
            ScarletItemIDSets.NoGeneralExecutionProgressDraw[Type] = true;
            ScarletItemIDSets.ForceToCustomExecute[Type] = true;
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.Donator);
        }
        public override void ExSD()
        {
            Item.damage = 1336;
            Item.useTime = Item.useAnimation = 18;
            Item.knockBack = 4.5f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 16f;
            Item.shoot = ProjectileType<TheGreatDipperHeldProj>();
            Item.SetUpRarityPrice(ItemRarityID.Green);
            Item.SetUpNoUseGraphicItem(true);
            Item.HJScarlet().OwnerName = "冰川咲";
            Item.HJScarlet().ItemBelongTo = EnumItemOwner.Donator;

        }
        public override bool CanShoot(Player player)
        {
            return !player.HasProj(Item.shoot);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool exe = player.GetExecutionSrike();
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.HJScarlet().HasExecutionMechanic = true;
            proj.HJScarlet().ExecutionStrike = exe;
            ((TheGreatDipperHeldProj)proj.ModProjectile).BeginTargetRotation = dir.ToRotation();
            ((TheGreatDipperHeldProj)proj.ModProjectile).Flip = Main.rand.NextBool();

            return false;
        }
        public override void HoldItem(Player player)
        {
            player.HJScarlet().tacticalExecution = true;
            bool noSevenStar = player.HasProj<TheGreatDipperDipper>() && player.HasProj<TheGreatDipperDipperStar>();
            if (noSevenStar || player.whoAmI != Main.myPlayer)
                return;
            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.MountedCenter, Vector2.Zero, ProjectileType<TheGreatDipperDipper>(), 0, 0, player.whoAmI);
            proj.Center = player.MountedCenter - player.direction * 120f * Vector2.UnitX;
        }
    }
}
