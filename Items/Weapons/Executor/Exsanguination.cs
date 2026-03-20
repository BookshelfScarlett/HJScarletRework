using ContinentOfJourney.Items;
using HJScarletRework.Globals.Executor;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Executor;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class Exsanguination : ExecutorWeaponClass
    {
        public override int ExecutionTime => 150;
        public override float ExecutionStrikeDamageMult => 50f;
        public override void ExSD()
        {
            Item.width = 172;
            Item.height = 72;
            Item.damage = 14;
            Item.useTime =  Item.useAnimation =20;
            Item.knockBack = 5f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = RarityType<DisasterRarity>();
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.shoot = ProjectileType<ExsanguinationHeldProj>();
            Item.shootSpeed = 12f;
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                DisasterRarity.DrawRarity2(line);
                return false;
            }
            if (line.Mod == "Terraria" && line.Name == "Damage")
            {
                DisasterRarity.DrawMisc(line);
                return false;
            }
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
                AddIngredient<ClockworkMinigun>().
                AddIngredient(ItemID.SDMG).
                AddIngredient(ItemID.FragmentVortex, 30).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
