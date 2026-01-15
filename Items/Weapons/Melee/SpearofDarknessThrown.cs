using ContinentOfJourney.Items;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class SpearofDarknessThrown : ThrownSpearClass
    {
        public override string Texture => HJScarletItemProj.Item_SpearofDarknessThrown.Path;
        public int StrikeTime = 0;
        public override void SetStaticDefaults() => Type.ShimmerEach<SpearOfDarkness>();
        public override void ExSD()
        {
            Item.damage = 34;
            Item.useTime = Item.useAnimation = 24;
            Item.UseSound = SoundID.Item103;
            Item.knockBack = 12f;
            Item.shootSpeed = 14f;
            Item.shoot = ProjectileType<SpearofDarknessThrownProj>();
        }
        public override Color MainTooltipColor => Color.MediumPurple;
    }
}
