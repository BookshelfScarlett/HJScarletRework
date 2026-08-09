using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.Misc
{
    public class SickleAndTorch : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 6;
        public bool Flip = true;
        public float Time = 1;
        public override void ExSD()
        {
            Item.SetUpNoUseGraphicItem(true);
            Item.SetUpRarityPrice(ItemRarityID.Blue);
            Item.damage = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 21;
            Item.shoot = ProjectileType<SickleAndTorchSickle>();
            Item.shootSpeed = 7;
            Item.UseSound = null;
            Item.knockBack = 2.7f;
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasProj(Item.shoot) && !player.HasProj<SickleAndTorchTorchToss>();
        }
        public override float UseSpeedMultiplier(Player player)
        {
            if (Time <= 2)
                return 0.7f;
            else
                return base.UseSpeedMultiplier(player);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool exe = player.GetExecutionSrike();
            if (Time <= 2)
            {
                Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
                Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
                proj.HJScarlet().HasExecutionMechanic = true;
                proj.HJScarlet().ExecutionStrike = exe;
                ((SickleAndTorchSickle)proj.ModProjectile).BeginTargetRotation = dir.ToRotation();
                ((SickleAndTorchSickle)proj.ModProjectile).Flip = Flip;
            }
            else
            {
                Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
                ScarletSound(SoundID.DD2_BetsyFireballShot, position, 0.4f, 1, 0.1f, .21f);
                //这里会把镰刀放在玩家背后
                Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
                ((SickleAndTorchSickle)proj.ModProjectile).BeginTargetRotation = dir.ToRotation();
                ((SickleAndTorchSickle)proj.ModProjectile).Flip = Flip;
                proj.ai[2] = 1;
                //这个射弹用于绘制火焰的轨迹
                proj = Projectile.NewProjectileDirect(source, position, velocity, ProjectileType<SickleAndTorchTorchToss>(), 0, knockback, player.whoAmI);
                proj.HJScarlet().HasExecutionMechanic = true;
                proj.HJScarlet().ExecutionStrike = exe;
                ((SickleAndTorchTorchToss)proj.ModProjectile).BeginTargetRotation = dir.ToRotation();
                ((SickleAndTorchTorchToss)proj.ModProjectile).Flip = Flip;
                //这个才是实际丢出的火把
                proj = Projectile.NewProjectileDirect(source, position, velocity, ProjectileType<SickleAndTorchTorch>(), damage, knockback, player.whoAmI);
                proj.rotation = RandRotTwoPi;
                proj.HJScarlet().HasExecutionMechanic = true;
                proj.HJScarlet().ExecutionStrike = exe;
                proj.ai[1] = Flip.ToDirectionInt() * player.direction;
                Time = 0;
                if (exe)
                {
                    proj.extraUpdates += 1;
                    player.RemoveExecutionProgress(Type);
                }
            }
            Time += 1;
            Flip = !Flip;
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Sickle).
                AddIngredient(ItemID.TikiTorch).
                AddIngredient(ItemID.RopeCoil).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
