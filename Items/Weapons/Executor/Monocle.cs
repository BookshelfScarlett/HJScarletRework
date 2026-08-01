using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class Monocle : ExecutorWeaponClass
    {
        public static int ExecutionPenetrate = 15;
        public static float ExecutionDamageMult = 1.5f;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Firearm;
        public override void ExSSD()
        {
        }
        public override void ExSD()
        {
            Item.damage = 1324;
            Item.shootSpeed = 19;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.SetUpNoUseGraphicItem(true);
            Item.HJScarlet().NotFinished = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = null;
            Item.useTime = Item.useAnimation = 45;
            Item.shoot = ProjectileType<MonocleHeldProj>();
        }
        public override bool CanShoot(Player player)
        {
            return !player.HasProj(Item.shoot);
        }
        public override void HoldItem(Player player)
        {
            if (player.HasProj(Item.shoot) || Main.dedServ)
                return;


        }
    }
}
