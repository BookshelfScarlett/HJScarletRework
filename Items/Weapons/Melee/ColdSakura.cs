using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class ColdSakura : ThrownSpearClass
    {
        public override bool NotHomewardJourneySpear => true;
        public override bool HasLegendary => true;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.BonusAttackSpeedMultiplier[Type] = 0.33f;
        }
        public override void ExSD()
        {
            Item.damage = 210;
            Item.knockBack = 12f;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.rare = RarityType<SakuraRarity>();
            Item.shoot = ProjectileType<ColdSakuraProj>();
            Item.UseSound = HJScarletSounds.Misc_KnifeToss[0] with { Volume = 0.6f,Pitch = 0.5f, PitchVariance = 0.1f, MaxInstances = 0 };
            Item.shootSpeed = 16f;
        }
        public override void HoldItem(Player player)
        {
            base.HoldItem(player);
        }
        public override Color MainTooltipColor => Color.LightPink;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //生成位置会在玩家的后方且有一定的偏移
            //为适配这一攻击模组，冷樱的hitbox会被有意增大
            Vector2 vel = velocity;
            Vector2 mouseVec = player.ToMouseVector2();
            Vector2 spawnPos = player.Center - mouseVec.ToRandVelocity(ToRadians(30f), 80f, 86f) - mouseVec * Main.rand.NextFloat(40f,60f);
            //这里只会取水平分量的速度
            Projectile proj = Projectile.NewProjectileDirect(source, spawnPos, vel, Item.shoot, damage, knockback,player.whoAmI);
            return false;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if(line.Name == "ItemName" && line.Mod == "Terraria")
            {
                SakuraRarity.DrawRarity(line);
                return false;
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
    }
}
