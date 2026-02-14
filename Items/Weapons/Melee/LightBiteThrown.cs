using ContinentOfJourney.Items;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class LightBiteThrown : ThrownSpearClass
    {
        public override void SetStaticDefaults() => Type.ShimmerEach<LightBite>();
        public override string Texture => GetInstance<LightBite>().Texture;
        public override void ExSD()
        {
            Item.damage = 104;
            Item.useTime = Item.useAnimation = 25;
            Item.knockBack = 12f;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.shoot = ProjectileType<LightBiteThrownProj>();
            Item.shootSpeed = 16;
        }
        public override Color MainTooltipColor => Color.DarkOrange;
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 8;
    }
}
