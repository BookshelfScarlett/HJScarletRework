using ContinentOfJourney.Items.Flamethrowers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Ranged
{
    public class TerraFlamethrower : HJScarletWeapon
    {
        public override ClassCategory Category => ClassCategory.Ranged;
        public override void ExSD()
        {
            Item.width = 172;
            Item.height = 72;
            Item.damage = 60;
            Item.useTime = Item.useAnimation = 6;
            Item.knockBack = 5f;
            Item.SetUpRarityPrice(ItemRarityID.LightRed);
            Item.SetUpNoUseGraphicItem(true, false);
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileType<TerraFlamethrowerHeldProj>();
            Item.shootSpeed = 12f;
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasProj(Item.shoot) && player.HJScarlet().heldProjReUseTime == 0;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, ai0: 9);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FT7SporeGun>().
                AddIngredient(ItemID.BrokenHeroSword).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
