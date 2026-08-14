using ContinentOfJourney.Items;
using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor.ColdSteel
{
    public class StormSaber : ExecutorWeaponClass
    {
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.ColdSteel;
        public override int ExecutionProgress => 10;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.FateWhite);
        }
        public override void ExSD()
        {
            Item.damage = 360;
            Item.useTime = Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 16f;
            Item.shoot = ProjectileType<StormSaberHeldProj>();
            Item.knockBack = 4f;
            Item.SetUpRarityPrice(ItemRarityID.Green);
            Item.SetUpNoUseGraphicItem(true);
        }
        public override void PostDrawTooltipLine(DrawableTooltipLine line)
        {
            base.PostDrawTooltipLine(line);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool exe = player.GetExecutionSrike();
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.HJScarlet().HasExecutionMechanic = true;
            proj.HJScarlet().ExecutionStrike = exe;
            ((StormSaberHeldProj)proj.ModProjectile).BeginTargetRotation = dir.ToRotation();
            ((StormSaberHeldProj)proj.ModProjectile).Flip = Main.rand.NextBool();
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TornadoScythe>().
                AddIngredient<SoulofBlight>(5).
                AddIngredient<DeepBar>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
