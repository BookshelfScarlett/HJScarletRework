using ContinentOfJourney.Items;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace HJScarletRework.Items.Weapons.Executor
{
    public class Exsanguination : ExecutorWeaponClass
    {
        public override int ExecutionProgress => 500;
        public override float ExecutionStrikeDamageMult => 1;
        public override ExecutorWeaponType ExecutorWeaponType => ExecutorWeaponType.Firearm;
        public override void ExSSD()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.ScarletRed);
        }
        public override void ExSD()
        {
            Item.damage = 33;
            Item.useTime = Item.useAnimation = 20;
            Item.knockBack = 5f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.shoot = ProjectileType<ExsanguinationHeldProj>();
            Item.shootSpeed = 12f;
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        public override bool CanUseItem(Player player) => !player.HasProj(Item.shoot);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ChainGun).
                AddIngredient(ItemID.FragmentSolar,5).
                AddIngredient(ItemID.FragmentVortex, 5).
                AddIngredient(ItemID.FragmentNebula, 5).
                AddIngredient(ItemID.FragmentStardust, 35).
                AddTile(TileID.LunarBlockNebula).
                Register();
        }
    }
}
