using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.IDSets;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor.ColdSteel
{
    public class TitaniumBattleShovel :ExecutorWeaponClass
    {
        public override int ExecutionProgress => 9;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.ColdSteel;
        public override void ExSSD()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            ScarletItemIDSets.CountAsWeapon[Type] = true;
            ScarletItemIDSets.IsHeldProjItem[Type] = true;
        }
        public override void ExSD()
        {
            Item.damage = 120;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5;
            Item.useTime = Item.useAnimation = 25;
            Item.SetUpNoUseGraphicItem(true);
            Item.SetUpRarityPrice(ItemRarityID.LightRed);
            Item.shootSpeed = 16f;
            Item.shoot = ProjectileType<TitaniumBattleShovelHeldProj>();
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool CanShoot(Player player)
        {
            return !player.HasProj(Item.shoot);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            proj.HJScarlet().HasExecutionMechanic = true;
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.GravediggerShovel).
                AddIngredient(ItemID.TitaniumBar, 12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
