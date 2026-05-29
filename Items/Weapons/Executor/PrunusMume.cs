using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Rarity.RarityShiny;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class PrunusMume: ExecutorWeaponClass
    {
        public override float ExecutionStrikeDamageMult => 1f;
        public override int ExecutionProgress => 32;
        public override void ExSD()
        {
            Item.damage = 210;
            Item.knockBack = 1.5f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shootSpeed = 19f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ProjectileType<PrunusMumeProj>();
            Item.UseSound = SoundID.Item1;
            Item.useTime = Item.useAnimation = 28;
            Item.SetUpRarityPrice(ItemRarityID.Red);
        }
    }
}
