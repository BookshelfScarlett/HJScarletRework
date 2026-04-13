using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Instances;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class TerraSpear : ThrownSpearClass
    {
        public override bool NotHomewardJourneySpear => true;
        public override bool HasLegendary => false;
        public override void ExSD()
        {
            Item.width = Item.height = 92;
            Item.damage = 65;
            Item.knockBack = 8f;
            Item.useTime = Item.useAnimation = 55;
            Item.shootSpeed = 18f;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = HJScarletSounds.Hammer_Shoot[0] with {Pitch = -0.5f, MaxInstances = 0, Volume = 0.8f};
            Item.shoot = ProjectileType<TerraSpearProj>();

        }
        public override Color MainTooltipColor => Color.GreenYellow;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CryoblazeHymn>().
                AddRecipeGroup(HJScarletRecipeGroup.AnySpearofDarkness).
                AddIngredient(ItemID.BrokenHeroSword).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

    }
}
