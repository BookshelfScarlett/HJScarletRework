using ContinentOfJourney.Items;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class DeepToneThrown : ThrownSpearClass
    {
        public override string Texture => GetInstance<DeepTone>().Texture;
        public override void SetStaticDefaults() => Type.ShimmerEach<DeepTone>();
        public override void ExSD()
        {
            Item.damage = 34;
            Item.useTime = Item.useAnimation = 24;
            Item.UseSound = HJScarletSounds.Misc_KnifeToss[0] with { Pitch = 0.5f, PitchVariance = 0.1f, MaxInstances = 0 };
            Item.knockBack = 12f;
            Item.shootSpeed = 14f;
            Item.shoot = ProjectileType<DeepToneThrownProj>();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string path = Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}.Tooltip");
            tooltips.ReplaceAllTooltip(path, Color.LightSeaGreen);
        }
    }
}
