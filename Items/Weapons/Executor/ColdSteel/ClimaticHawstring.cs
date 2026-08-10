using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.ColdSteel
{
    public class ClimaticHawstring : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 40;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.ColdSteel;
        public override void ExSD()
        {
            Item.damage = 60;
            Item.useTime = Item.useAnimation = 20;
            Item.knockBack = 5f;
            Item.SetUpRarityPrice(ItemRarityID.LightRed);
            Item.SetUpNoUseGraphicItem(true, false);
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileType<ClimaticHawstringProj>();
            Item.shootSpeed = 12f;

        }
        public override bool CanShoot(Player player) => false;
        public override void HoldItem(Player player)
        {
            if (player.GetExecutionSrike())
            {
                player.HJScarlet().climaticHawstringLaserCounter = 30;
                player.RemoveExecutionProgress(Type);
            }
            if (player.HasProj(Item.shoot))
                return;
            int damage = (int)player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(Item.damage);
            Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, Item.shoot, damage, Item.knockBack, player.whoAmI, ai0: 9);

        }
    }
}
