using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class EthrealSwordfish : ThrownSpearClass
    {
        public override bool NotHomewardJourneySpear => true;
        public override void ExSD()
        {
            Item.damage = 190;
            Item.knockBack = 2f;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.UseSound = HJScarletSounds.Misc_KnifeToss[0] with { Volume = 0.6f, Pitch = 0.5f, PitchVariance = 0.1f, MaxInstances = 0 };
            Item.shootSpeed = 16f;
        }
        public override Color MainTooltipColor => Color.Lerp(Color.Gold, Color.LightGoldenrodYellow, .3f);
    }
}
