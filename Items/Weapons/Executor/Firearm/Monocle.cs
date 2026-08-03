using ContinentOfJourney.Items.Material;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.Firearm
{
    public class Monocle : ExecutorWeaponClass
    {
        public static int ExecutionPenetrate = 15;
        public static float ExecutionDamageMult = 1.5f;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Firearm;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.FateWhite);
        }
        public override void ExSD()
        {
            Item.damage = 1324;
            Item.shootSpeed = 19;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.SetUpNoUseGraphicItem(true);
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = null;
            Item.knockBack = 7f;
            Item.useTime = Item.useAnimation = 60;
            Item.shoot = ProjectileType<MonocleHeldProj>();
            Item.HJScarlet().borderlandWeapon = true;
        }
        public override bool CanShoot(Player player)
        {
            return !player.HasProj(Item.shoot);
        }
        public override void HoldItem(Player player)
        {
            if (player.HasProj(Item.shoot) || Main.dedServ)
                return;
            int projDamage = (int)player.GetTotalDamage<ExecutorDamageClass>().ApplyTo(Item.damage);
            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, Item.shoot, 0, Item.knockBack, player.whoAmI);
            proj.originalDamage = projDamage;
            proj.HJScarlet().HasExecutionMechanic = true;
            proj.netUpdate = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SniperRifle).
                AddIngredient(ItemID.ShadowbeamStaff).
                AddIngredient<CubistBar>(10).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
