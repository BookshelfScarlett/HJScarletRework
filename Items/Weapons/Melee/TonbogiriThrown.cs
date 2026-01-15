using ContinentOfJourney.Items;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class TonbogiriThrown : ThrownSpearClass
    {
        public override string Texture => GetInstance<Tonbogiri>().Texture;
        public float CurrentAttackSpeedBonues = 0f;
        public override void SetStaticDefaults()
        {
            Type.ShimmerEach<Tonbogiri>();
            ItemID.Sets.BonusAttackSpeedMultiplier[Type] = 1.4f;
        }
        public override void ExSD()
        {
            Item.damage = 106;
            Item.useTime = Item.useAnimation = 20;
            Item.knockBack = 12f;
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.shootSpeed = 16;
            Item.channel = true;
            //这里的shoot是为了适配weaponoutlite。实际上我们不会直接shoot这个东西
            Item.shoot = ProjectileType<TonbogiriThrownProj>();
        }
        public override Color MainTooltipColor => Color.LightBlue;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
