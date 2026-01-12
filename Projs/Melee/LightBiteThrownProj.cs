using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class LightBiteThrownProj : ThrownSpearProjClass
    {
        public override string Texture => GetInstance<LightBiteThrown>().Texture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
        }
        public override void ExSD()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 10;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.light = 0.5f;
            //火焰
            for (int i = 0; i < 2; i++)
            {
                Color Firecolor2 = Color.Lerp(Color.DarkGoldenrod, Color.DarkOrange, Main.rand.NextFloat(0, 1));
                Vector2 fireOffset = Projectile.rotation.ToRotationVector2() * 40f + Main.rand.NextVector2Circular(4f, 4f);
                new Fire(Projectile.Center - fireOffset, Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0.2f, 1.2f) * 1.2f, Firecolor2, 15, Main.rand.NextFloat(TwoPi), 1, 0.15f * Projectile.Opacity).SpawnToPriorityNonPreMult();
            }
            //挥发性粒子
            Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(11, 11) - Projectile.rotation.ToRotationVector2() * 20f;
            Color Firecolor = Color.Lerp(Color.Gold, Color.Yellow, Main.rand.NextFloat(0, 1));
            new TurbulenceShinyOrb(spawnPos, 1f, Firecolor, 40, 0.20f * Projectile.Opacity, Main.rand.NextFloat(TwoPi)).Spawn();

        }
        SpriteBatch SB { get => Main.spriteBatch; }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetTexture(out Texture2D projTex, out Vector2 orig, out Vector2 drawPos);

            int length = Projectile.oldPos.Length;
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            for (int i = 0; i < length; i++)
            {
                float rads = (float)i / length;
                Color drawColor = (Color.Lerp(Color.Orange, Color.Gold, rads) with { A = 0 }) * 0.9f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                Main.spriteBatch.Draw(star, Projectile.oldPos[i] + Projectile.PosToCenter() + Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2) * 1f, null, drawColor * Clamp(Projectile.velocity.Length(), 0, 1), Projectile.oldRot[i] + PiOver2, star.Size() / 2, Projectile.scale * new Vector2(0.8f, 1.5f), 0, 0);
            }
            //绘制发光边缘
            for (int i =0; i < 8; i++)
            {
                SB.Draw(projTex, drawPos + ToRadians(i * 60f).ToRotationVector2() * 2f, null, Color.White with { A = 0 }, Projectile.rotation + PiOver4, orig, Projectile.scale, 0, 0f);
            }
            //绘制射弹
            for (int i = 0; i < Projectile.oldPos.Length;i++)
            {
                Vector2 trailPos = drawPos - Projectile.velocity * i * 0.7f;
                float faded = MathF.Pow(1 - i / (float)Projectile.oldPos.Length, 2);
                Color trailColor = Color.White * faded;
                SB.Draw(projTex, trailPos, null, trailColor, Projectile.oldRot[i] + PiOver4, orig, Projectile.scale, 0, 0);
            }
            SB.Draw(projTex, drawPos, null, Color.White, Projectile.rotation + PiOver4, orig, Projectile.scale, 0, 0);
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //发射粒子
            for (float i = 0; i < 12f; i += 1f)
            {
                Dust fireDust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24), DustID.OrangeTorch);
                fireDust.velocity += Projectile.velocity;
                fireDust.noGravity = true;
                fireDust.scale *= 2;
            }
            Vector2 spawnPos = Projectile.Center;
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, oldVelocity.SafeNormalize(Vector2.UnitX) * 12f, ProjectileType<LightBiteArrow>(), Projectile.damage, Projectile.knockBack);
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //发射粒子
            for (float i = 0; i < 12f; i += 1f)
            {
                Dust fireDust = Dust.NewDustDirect(Projectile.Center, 16, 16, DustID.Torch);
                fireDust.velocity += Projectile.velocity * Main.rand.NextFloat(1.2f, 1.5f);
                fireDust.noGravity = true;
                fireDust.scale *= 2;
            }
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.SafeDir() * 4f , ProjectileType<LightBiteArrow>(), Projectile.damage, Projectile.knockBack);
            proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
            proj.penetrate = 1;
            proj.ai[0] = 60;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0.90f;
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
        }
    }
}
