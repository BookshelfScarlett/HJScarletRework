using HJScarletRework.Executor;
using HJScarletRework.Projs.Executor;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Executor
{
    public class BlazingStriker : ExecutorWeaponClass
    {
        public override float FocusStrikeDamageMultipler => 0.5f;
        public override int FocusStrikeTime => 6;
        public override void ExSD()
        {
            Item.width = Item.height = 66;
            Item.damage = 44;
            Item.knockBack = 8f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shootSpeed = 16f;
            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ProjectileType<BlazingStrikerProj>();
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.AshWoodHammer).
                AddIngredient(ItemID.HellstoneBar, 10).
                AddTile(TileID.Hellforge).
                Register();
        }
    }
}
