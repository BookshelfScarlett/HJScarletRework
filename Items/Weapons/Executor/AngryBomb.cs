using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class AngryBomb : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 15;
        public override WeaponCategory WeaponCategory => WeaponCategory.Throw;
        public override void ExSD()
        {
            Item.damage = 16;
            Item.shootSpeed = 20;
            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.SetUpRarityPrice(ItemRarityID.Blue);
            Item.knockBack = 2f;
            Item.shoot = ProjectileType<AngryBombProj>();
            Item.UseSound = HJScarletSounds.Misc_KnifeTossAlt with { Pitch= 0.5f, Variants = [3] , Volume = 0.75f};
            Item.SetUpNoUseGraphicItem();
            Item.HJScarlet().ExecutionProj = ProjectileType<AngryBombExecution>();
        }
    }
}
