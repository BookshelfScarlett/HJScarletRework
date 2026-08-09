using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Graphics.Metaballs;
using HJScarletRework.Globals.IDSets;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.Misc
{
    public class TheSevenStar : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 7;
        public static int Damage = 34;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.Donator);
            ScarletItemIDSets.ForceToTacticalExecute[Type] = true;
            ScarletItemIDSets.NoGeneralExecutionProgressDraw[Type] = true;
        }
        public override void ExSD()
        {
            Item.knockBack = 4.5f;
            Item.damage = 36;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 16f;
            Item.shoot = ProjectileType<TheSevenStarHeldProj>();
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
            ((TheSevenStarHeldProj)proj.ModProjectile).BeginTargetRotation = dir.ToRotation();
            ((TheSevenStarHeldProj)proj.ModProjectile).Flip = Main.rand.NextBool();

            return false;
        }
        public override void HoldItem(Player player)
        {
            for (int i = 0; i < 160; i++)
            {
                ShinyStardust.SpawnCircle(player.Center.ToRandCirclePos(18), RandVelTwoPi(1.2f,34f), Main.rand.NextFloat(.9f, 1.1f) * .94f, 120);
            }
            bool noSevenStar = player.HasProj<TheSevenStarDipper>() && player.HasProj<TheSevenStarDipperStar>();
            if (noSevenStar || player.whoAmI != Main.myPlayer)
                return;
            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.MountedCenter, Vector2.Zero, ProjectileType<TheSevenStarDipper>(), 0, 0, player.whoAmI);
                proj.Center = player.MountedCenter - player.direction * 80f * Vector2.UnitX;
        }
    }
}
