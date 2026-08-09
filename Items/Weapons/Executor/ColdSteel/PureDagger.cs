using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.ColdSteel
{
    public class PureDagger : ExecutorWeaponClass
    {
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.ColdSteel;
        public static int DefenseAdd = 4;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.FateWhite);
        }
        public override void ExSD()
        {
            Item.damage = 36;
            Item.useTime = Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 16f;
            Item.shoot = ProjectileType<PureDaggerProj>();
            Item.SetUpRarityPrice(ItemRarityID.Green);
            Item.SetUpNoUseGraphicItem(true);
            Item.master = true;
        }
        public override bool CanShoot(Player player)
        {
            return !player.HasProj(Item.shoot) && !player.HasProj<PureDaggerExecution>();
            //return !player.HasProj<EndlessWarSmasher>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool exe = player.GetExecutionSrike();
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.HJScarlet().HasExecutionMechanic = true;
            proj.HJScarlet().ExecutionStrike = exe;
            ((PureDaggerProj)proj.ModProjectile).BeginTargetRotation = dir.ToRotation();
            ((PureDaggerProj)proj.ModProjectile).Flip = Main.rand.NextBool();
            //bool exe = player.GetExecutionSrike();
            //Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            //Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, ProjectileType<EndlessWarSmasher>(), damage, knockback, player.whoAmI);
            //proj.HJScarlet().HasExecutionMechanic = true;
            //proj.HJScarlet().ExecutionStrike = exe;
            //((EndlessWarSmasher)proj.ModProjectile).BeginTargetRotation = dir.ToRotation();
            //((EndlessWarSmasher)proj.ModProjectile).Flip = Main.rand.NextBool();

            return false;
        }
    }
}
