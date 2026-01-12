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
        public override void SetStaticDefaults() => Type.ShimmerEach<WildPointer>();
        internal int UseTime = 0;
        public override void ExSD()
        {
            Item.damage = 20;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item91 with { Volume = 0.7f, Pitch = 0.4f};
            Item.shoot = ProjectileType<WildPointerThrownProj>();
            Item.shootSpeed = 14;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //这里直接用MainMW应该没问题，多人同步后面再看看
            Vector2 direction = (Main.MouseWorld - player.MountedCenter).SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(ToRadians(15) * Main.rand.NextBool().ToDirectionInt()));
            Vector2 spawnPos = player.MountedCenter + direction * 25f + direction.RotatedBy(PiOver2 * Main.rand.NextBool().ToDirectionInt()) * Main.rand.NextFloat(15f * Main.rand.NextBool().ToDirectionInt());
            int ai0 = Main.rand.NextBool(5).ToInt();
            Projectile.NewProjectile(source, spawnPos, direction * Item.shootSpeed, type, damage, knockback, player.whoAmI, ai0);
            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string path = Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}.Tooltip");
            tooltips.ReplaceAllTooltip(path, Color.SkyBlue);
        }
    }
}
