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
            Item.damage = 106;
            Item.useTime = Item.useAnimation = 20;
            Item.knockBack = 12f;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.shoot = ProjectileType<LightBiteThrownProj>();
            Item.shootSpeed = 16;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 4;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string path = Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}.Tooltip");
            tooltips.ReplaceAllTooltip(path, Color.DarkOrange);
        }
    }
}
