using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class FierySpear : ThrownSpearClass
    {
        public override bool NotHomewardJourneySpear => true;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            Item.damage = 30;
            Item.useTime = Item.useAnimation = 35;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item45 with { MaxInstances = 0 };
            Item.shootSpeed = 11f;
            Item.shoot = ProjectileType<FierySpearProj>();
            Item.rare = ItemRarityID.Orange;
        }
        public override Color MainTooltipColor => Color.LightYellow;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Spear).
                AddIngredient(ItemID.HellstoneBar, 10).
                AddTile(TileID.Hellforge).
                Register();
        }
    }
}
