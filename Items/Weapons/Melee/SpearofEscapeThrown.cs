using ContinentOfJourney.Items;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class SpearofEscapeThrown : ThrownSpearClass
    {
        public override string Texture => GetInstance<SpearOfEscape>().Texture;
        public override bool HasLegendary => true;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void ExSD()
        {
            Item.damage = 426;
            Item.useTime = Item.useAnimation = 42;
            Item.knockBack = 12f;
            Item.UseSound = HJScarletSounds.SpearofEscape_Toss;
            Item.shootSpeed = 24;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ProjectileType<SpearofEscapeRider>();
        }
        public override bool AltFunctionUse(Player player) => true;
        public override Color MainTooltipColor => Color.SkyBlue;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
                proj.ai[0] = 1;
            }
            else
            {
                Vector2 ownerToSky = new Vector2(Lerp(player.ToClampMouseVector2().X, player.Center.X, 0.25f), player.Center.Y) + new Vector2(0, -500) - player.Center;
                Vector2 skyDir = (ownerToSky).ToSafeNormalize();
                Projectile.NewProjectileDirect(source, position, skyDir * Item.shootSpeed, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override void HoldItem(Player player)
        {

        }
    }
}
