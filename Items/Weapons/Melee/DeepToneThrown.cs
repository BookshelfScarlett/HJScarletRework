using ContinentOfJourney.Items;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class DeepToneThrown : ThrownSpearClass
    {
        public override string Texture => GetInstance<DeepTone>().Texture;
        public override void ExSD()
        {
            Item.damage = 71;
            Item.useTime = Item.useAnimation = 35;
            Item.UseSound = HJScarletSounds.Misc_KnifeToss[0] with { Pitch = 0.5f, PitchVariance = 0.1f, MaxInstances = 0 };
            Item.knockBack = 3f;
            Item.shootSpeed = 14f;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ProjectileType<DeepToneThrownProj>();
        }
        public override Color MainTooltipColor => Color.DarkOliveGreen;
    }
}
