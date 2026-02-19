using ContinentOfJourney.Items;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class CandLanceThrown : ThrownSpearClass
    {
        public override string Texture => GetInstance<Candlance>().Texture;
        public override void ExSD()
        {
            Item.damage = 26;
            Item.shoot = ProjectileType<CandLanceThrownProj>();
            Item.shootSpeed = 12f;
            Item.rare = ItemRarityID.Orange;
            Item.knockBack = 4f;
            Item.UseSound = HJScarletSounds.Misc_MagicStaffFire with { MaxInstances = 0, Pitch = 0.1f, Volume = 0.18f, PitchVariance = 0.2f};
            Item.useTime = Item.useAnimation = 40;
        }
        public override Color MainTooltipColor => Color.SkyBlue;
    }
}
