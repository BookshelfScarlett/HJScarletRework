using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class AbyssalWorld : ExecutorWeaponClass
    {
        public override int ExecutionTime => 40;
        public override void ExSD()
        {
            Item.width = Item.height = 72;
            Item.damage = 108;
            Item.useTime = Item.useAnimation = 35;
            Item.rare = ItemRarityID.Yellow;
            Item.shootSpeed = 18f;
            Item.shoot = ProjectileType<AbyssalWorldProj>();
        }
        public override bool CanUseItem(Player player) => !player.HasProj<AbyssalWorldProj>();
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return true;
        }
    }
}
