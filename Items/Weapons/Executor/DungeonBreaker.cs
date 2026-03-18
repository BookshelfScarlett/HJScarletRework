using HJScarletRework.Executor;
using HJScarletRework.Projs.Executor;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class DungeonBreaker: ExecutorWeaponClass
    {
        public override float FocusStrikeDamageMultipler => 0.5f;
        public override int FocusStrikeTime => 20;
        public override void ExSD()
        {
            Item.width = Item.height = 66;
            Item.damage = 44;
            Item.knockBack = 8f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shootSpeed = 12f;
            Item.useTime = Item.useAnimation = 38;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ProjectileType<DungeonBreakerFocusProj>();
            Item.rare = ItemRarityID.LightPurple;
        }
    }
}
