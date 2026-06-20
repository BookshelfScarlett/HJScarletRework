using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class EnchantedSwordfish : ThrownSpearClass
    {
        public override bool NotHomewardJourneySpear => true;
        public override void ExSD()
        {
            Item.damage = 190;
            Item.knockBack = 2f;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.shoot = ProjectileType<EnchantedSwordfishJavelin>();
            Item.UseSound = HJScarletSounds.Misc_KnifeToss[0] with { Volume = 0.6f, Pitch = 0.5f, PitchVariance = 0.1f, MaxInstances = 0 };
            Item.shootSpeed = 21f;
        }
        public override Color MainTooltipColor => Color.Lerp(Color.RoyalBlue, Color.SkyBlue,.3f);
    }
}
