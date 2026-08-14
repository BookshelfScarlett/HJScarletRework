using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.Assistance
{
    public class GhostKnife : ExecutorWeaponClass
    {
        public static int MarkGhostKnifeAttackSpeed = 30;
        public static int MarkGhostKnifeAttackDamage = 28;
        public static int LastTime = 30;
        public static int CritBonus = 10;
        public override int ExecutionProgress => 20;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Assistance;
        public override void ExSSD()
        {
            ExecutionDetail = [CritBonus + "%", LastTime];
        }
        public override void ExSD()
        {
            Item.shootSpeed = 16;
            Item.damage = MarkGhostKnifeAttackDamage * 2;
            Item.useTime = Item.useAnimation = MarkGhostKnifeAttackSpeed / 2;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ProjectileType<GhostKnifeProj>();
            Item.knockBack = 2;
            Item.HJScarlet().ExecutionProj = ProjectileType<GhostKnifeMark>();
            Item.HJScarlet().ForceAutomaticExecution = true;
            Item.SetUpRarityPrice(ItemRarityID.LightPurple);
            Item.SetUpNoUseGraphicItem();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.HJScarlet().HasExecutionMechanic = true;
            QuickSpawnMark(Type, Item.HJScarlet().ExecutionProj, player, source, position);
            return false;
        }
        public static void QuickSpawnMark(int weaponID, int markID, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position)
        {
            bool exe = player.GetExecutionSrike();
            if (!exe)
                return;
            //场上如果存在其余处死标记，立刻删除
            foreach (var p in Main.ActiveProjectiles)
            {
                if (p.active && p.owner == player.whoAmI && p.ModProjectile is KnifeMarkClass && p.type != markID)
                {
                    p.Kill();
                }
            }
            if (!player.HasProj(markID))
            {
                Vector2 targetVector = player.Center.GetNormalVector2(Main.MouseWorld);
                Projectile mark = Projectile.NewProjectileDirect(source, position - targetVector * 80f, Vector2.Zero, markID, 0, 0, player.whoAmI);
                player.RemoveExecutionProgress(weaponID);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpectreBar, 12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
