using ContinentOfJourney.Items;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class WildPointerThrown : ThrownSpearClass
    {
        public override string Texture => GetInstance<WildPointer>().Texture;
        internal int UseTime = 0;
        public override void ExSD()
        {
            Item.damage = 19;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item91 with { Volume = 0.7f, Pitch = 0.4f};
            Item.shoot = ProjectileType<WildPointerThrownProj>();
            Item.shootSpeed = 16;
            Item.knockBack = 2f;
        }
        public override Color MainTooltipColor => Color.SkyBlue;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //这里直接用MainMW应该没问题，多人同步后面再看看
            Vector2 direction = player.ToMouseVector2().ToRandVelocity(ToRadians(15));
            Vector2 spawnPos = player.MountedCenter + direction * 25f + direction.RotatedBy(PiOver2 * Main.rand.NextBool().ToDirectionInt()) * Main.rand.NextFloat(15f * Main.rand.NextBool().ToDirectionInt());
            int chance = 5;
            if (HJScarletMethods.HasFuckingCalamity)
                chance = 2;
            int ai0 = Main.rand.NextBool(chance).ToInt();
            Projectile proj = Projectile.NewProjectileDirect(source, spawnPos, direction * Item.shootSpeed, type, damage, knockback, player.whoAmI, ai0);
            return false;
        }
    }
}
