using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class JungleMadness: ExecutorWeaponClass
    {
        public override float ExecutionStrikeDamageMult => 1f;
        public override int ExecutionTime => 15;
        public override int ExecutionProj => base.ExecutionProj;
        public override void ExSD()
        {
            Item.width = Item.height = 66;
            Item.damage = 44;
            Item.knockBack = 8f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shootSpeed = 16f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ProjectileType<JungleMadnessProj>();
            Item.useTime = Item.useAnimation = 30;
            Item.SetUpRarityPrice(ItemRarityID.Blue);
        }
    }
}
