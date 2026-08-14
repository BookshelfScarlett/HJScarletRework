using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor.Assistance
{
    public class StreamingProtocol : ExecutorWeaponClass
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }

        public override int ExecutionProgress => 20;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Assistance;
        public override void ExSD()
        {
            Item.SetUpNoUseGraphicItem(true);
            Item.SetUpRarityPrice(ItemRarityID.Orange);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 24;
            Item.knockBack = 2;
            Item.UseSound = SoundID.Item152;
            Item.damage = 20;
            Item.shootSpeed = 12f;
            Item.shoot = ProjectileType<StarofHopeProj>();
        }
    }
}
