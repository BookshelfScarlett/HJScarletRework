using HJScarletRework.Assets.Registers;
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
            Item.damage = 78;
            Item.knockBack = 8f;
            Item.useTime = Item.useAnimation = 70;
            Item.shootSpeed = 16f;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = HJScarletSounds.Hammer_Shoot[0] with {Pitch = -0.5f, MaxInstances = 0, Volume = 0.8f};
            Item.shoot = ProjectileType<TerraSpearProj>();

        }
        public override Color MainTooltipColor => Color.GreenYellow;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CryoblazeHymn>().
                AddIngredient(ItemID.ChlorophyteBar, 10).
                AddIngredient(ItemID.BrokenHeroSword).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

    }
}
