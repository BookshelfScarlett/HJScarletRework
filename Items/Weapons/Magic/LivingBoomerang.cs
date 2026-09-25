using ContinentOfJourney.Items;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Database.Enums;
using HJScarletRework.Globals.Database.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Materials;
using HJScarletRework.Projs.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Magic
{
    public class LivingBoomerangMagic : HJScarletWeapon
    {
        public override string Texture => GetInstance<LivingBoomerang>().Texture;
        public override EnumDamageClass Category => EnumDamageClass.Magic;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, ShinyRarityType.Life);
            Item.staff[Type] = true;
        }
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.HJScarlet().drawBuffIconAndDetail = true;
            Item.damage = 456;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.mana = 15;
            Item.useTime = Item.useAnimation = 30;
            Item.shoot = ProjectileType<LivingBoomerangMagicHeldProj>();
            Item.shootSpeed = 16f;
            Item.SetUpNoUseGraphicItem(true);
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasProj(Item.shoot);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LivingBoomerang>().
                AddIngredient<CrownofSilveryLight>(15).
                AddTile(FinalAnvilTile).
                Register();
        }
    }
}
