using ContinentOfJourney.Items;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class LightBiteThrown : ThrownSpearClass
    {
        public override string Texture => GetInstance<LightBite>().Texture;
        int UseTime = HJScarletMethods.HasFuckingCalamity ? 12 : 25;
        public override void ExSD()
        {
            Item.damage = 104;
            Item.useTime = Item.useAnimation = UseTime;
            Item.knockBack = 12f;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.shoot = ProjectileType<LightBiteThrownProj>();
            Item.shootSpeed = 16;
        }
        public override Color MainTooltipColor => Color.DarkOrange;
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 8;
    }
}
