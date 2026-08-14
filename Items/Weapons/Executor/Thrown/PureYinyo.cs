using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.Thrown
{
    public class PureYinyo : ExecutorWeaponClass
    {
        public int AlterType = -1;
        public override int ExecutionProgress => 12;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Throw;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.FateWhite);
        }
        public override void ExSD()
        {
            Item.damage = 50;
            Item.SetUpRarityPrice(ItemRarityID.LightPurple);
            Item.SetUpNoUseGraphicItem();
            Item.knockBack = 2f;
            Item.useTime = Item.useAnimation = 31;
            Item.shootSpeed = 19f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ProjectileType<PureYinyoBlack>();
            Item.HJScarlet().ExecutionProj = ProjectileType<PureYinyoExecution>();
            Item.UseSound = HJScarletSounds.Blunt_Swing;

        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //初始化。
            bool useExecution = player.GetExecutionSrike();
            int projID = Item.HJScarlet().ExecutionProj != -1 && useExecution ? Item.HJScarlet().ExecutionProj : AlterType != 0 ? ProjectileType<PureYinyoWhite>() : type;
            if (useExecution)
            {
                Projectile proj = Projectile.NewProjectileDirect(source, position, -Vector2.UnitY * 32f, projID, damage, knockback, player.whoAmI);
                proj.HJScarlet().ExecutionStrike = true;
                player.RemoveExecutionProgress(Type);
                projID = AlterType != 0 ? ProjectileType<PureYinyoWhite>() : type;
            }
            {
                Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, projID, damage, knockback, player.whoAmI);
                proj.HJScarlet().HasExecutionMechanic = true;
                if (AlterType != 0)
                    AlterType = 0;
                else
                    AlterType = -1;
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LightShard, 2).
                AddIngredient(ItemID.DarkShard, 2).
                AddIngredient(ItemID.SoulofLight, 5).
                AddIngredient(ItemID.SoulofNight, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
