using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class SpectreStaff : ExecutorWeaponClass
    {
        public bool AlterVersion = false;
        public override int ExecutionTime => 20;
        public override int ExecutionProj => ProjectileType<SpectreStaffExecution>();
        public override void ExSD()
        {
            Item.damage = 60; 
            Item.SetUpRarityPrice(ItemRarityID.LightPurple);
            Item.SetUpNoUseGraphicItem(true);
            Item.shootSpeed = 16;
            Item.knockBack = 1;
            Item.SetUpItemUseTime(ItemUseStyleID.Shoot, 40);
        }
        public override bool CanShoot(Player player)
        {
            return !player.HasProj<SpectreStaffHeldProj>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //初始化。
            bool useExecution = player.CheckExecution(Type);
            int projID = ExecutionProj != -1 && useExecution ? ExecutionProj : ProjectileType<SpectreStaffHeldProj>();
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, projID, damage, knockback, player.whoAmI);
            proj.HJScarlet().HasExecutionMechanic = true;
            player.HJScarlet().CanExecution = false;
            if (useExecution)
            {
                proj.HJScarlet().ExecutionStrike = true;
                player.RemoveExecutionProgress(Type);
            }
            return false;
        }
        public override void HoldItem(Player player)
        {
            player.HJScarlet().tacticalExecution = true;
            if (player.HJScarlet().tacticalExecutionManual)
            {
                Asset<Texture2D> tex = AlterVersion ? HJScarletItemProj.SpectreStaff.Texture : HJScarletItemProj.SpectreStaffBlade.Texture;
                TextureAssets.Item[Type] = tex;
                player.HJScarlet().tacticalExecutionManual = false;
                player.RemoveExecutionProgress(Type);
            }
            if(!AlterVersion && player.whoAmI == Main.myPlayer && !player.HasProj<SpectreStaffHeldProj>())
            {
                float anchorPosX = player.MountedCenter.X - player.direction * 80f;
                Vector2 spawnPos = new Vector2(anchorPosX, player.MountedCenter.Y - 250f);
                int projDamage = (int)player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(Item.damage);
                Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), spawnPos, Vector2.Zero, ProjectileType<SpectreStaffHeldProj>(), projDamage, Item.knockBack, player.whoAmI);
                proj.originalDamage = projDamage;
                proj.netUpdate = true;
            }
        }
    }
}
