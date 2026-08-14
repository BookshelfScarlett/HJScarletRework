using ContinentOfJourney;
using ContinentOfJourney.Items;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.IDSets;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using HJScarletRework.Projs.Executor;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor.ColdSteel
{
    public class FrostoftheStorm : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 12;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.ColdSteel;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.Frost);
            ScarletItemIDSets.GrantsBoosterAfterSon[Type] = true;
        }
        public override void ExSD()
        {
            Item.damage = 1276;
            Item.crit = 20;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.SetUpNoUseGraphicItem(true, true);
            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = null;
            Item.knockBack = 2f;
            Item.shoot = ProjectileType<FrostoftheStormHeldProj>();
            Item.shootSpeed = 16;
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasProj(Item.shoot) && !player.HasProj<FrostoftheStormExecution>() && !player.HasProj<FrostoftheStormChargeProj>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.HJScarlet().HasExecutionMechanic = true;
            ((FrostoftheStormHeldProj)proj.ModProjectile).BeginTargetRotation = dir.ToRotation();
            ((FrostoftheStormHeldProj)proj.ModProjectile).Flip = true;
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Frostbrand).
                AddIngredient<Frostgrief>().
                AddIngredient<CrownofSilveryLight>(15).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
