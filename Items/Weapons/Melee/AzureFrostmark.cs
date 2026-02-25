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
            Item.damage = 27;
            Item.rare = ItemRarityID.Green;
            Item.useTime = Item.useAnimation = 40;
            Item.knockBack = 4f;
            Item.shootSpeed = 17f;
            Item.shoot = ProjectileType<AzureFrostmarkProj>();
            Item.UseSound = SoundID.Item1;
        }
        public override Color MainTooltipColor => Color.RoyalBlue;
    }
}
