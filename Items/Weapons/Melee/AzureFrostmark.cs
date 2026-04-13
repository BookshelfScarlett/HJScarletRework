using HJScarletRework.Assets.Registers;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class AzureFrostmark : ThrownSpearClass
    {
        public override bool NotHomewardJourneySpear => true;
        public override void ExSD()
        {
            Item.damage = 22;
            Item.rare = ItemRarityID.Green;
            Item.useTime = Item.useAnimation = 40;
            Item.knockBack = 4f;
            Item.shootSpeed = 11f;
            Item.shoot = ProjectileType<AzureFrostmarkProj>();
            Item.UseSound = HJScarletSounds.HymnFireball_Release with { MaxInstances = 1, Pitch = -0.20f , PitchVariance = 0.15f, Volume = 0.8f};
        }
        public override Color MainTooltipColor => Color.RoyalBlue;
    }
}
