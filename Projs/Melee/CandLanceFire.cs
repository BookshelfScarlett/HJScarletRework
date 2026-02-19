using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.NET.StringTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Intrinsics;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class CandLanceFire : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletItemProj.Proj_CandLanceFire.Path;
        public override ClassCategory Category => ClassCategory.Melee;
        public ref float AttackTimer => ref Projectile.ai[0];
        
        public ref float RandomValue => ref Projectile.ai[2];
        public ref float DrawGlowScale => ref Projectile.localAI[0];
        public float Osci = 0.025f;
        public float AttackDelay = 4;
        public bool IsHitTile = false;
        public float SpawnTime = 0;
        float RotFix = ToRadians(38);
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.noEnchantmentVisuals = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
        }
        private int SpawnBeamCounts = 3;
        public override void AI()
        {
            if(Projectile.HJScarlet().FirstFrame && HJScarletMethods.HasFuckingCalamity)
            {
                SpawnBeamCounts = 6;
            }
            //本质上控的是火焰方向，不过写在这里也为了方便一些操作
            IsHitTile = Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height);
            Projectile.velocity *= !IsHitTile ? 0.92f : 0.74f;
            DrawGlowScale = Clamp(Math.Abs(MathF.Sin(Osci / 4)) * 1.2f, 1f, 1.2f);
            if (Projectile.GetTargetSafe(out NPC target, Projectile.HJScarlet().GlobalTargetIndex, false))
                Projectile.position.X = Lerp(Projectile.position.X + RandomValue, target.position.X, 0.2f);
            //用粒子点燃
            Projectile.rotation = Projectile.velocity.ToRotation();
            IgniteFire();
            if (Projectile.velocity.Length() > 4f)
                return;
            AttackTimer += 1;
            if (AttackTimer < AttackDelay * (SpawnBeamCounts + 1))
            {
                if (AttackTimer % AttackDelay == 0)
                {
                    //向下方向，生成一个小型的鬼魂粒子
                    SpawnTime += 1f;
                    int fireDamage = (int)(Projectile.damage * (SpawnTime / SpawnBeamCounts));
                    if (HJScarletMethods.HasFuckingCalamity)
                        fireDamage = Projectile.damage;
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitY.ToRandVelocity(PiOver4) * Main.rand.NextFloat(3f, 4f), ProjectileType<CandLanceBeam>(), fireDamage, Projectile.knockBack, Owner.whoAmI);
                    SoundEngine.PlaySound(HJScarletSounds.Evolution_Thrown with { Volume = 0.7f, MaxInstances = 0, Pitch = 0.7f }, Projectile.Center);
                }
            }
            else
            {
                Projectile.Opacity -= 0.02f;
                if (Projectile.Opacity == 0)
                    Projectile.Kill();
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!IsHitTile)
            {
                IsHitTile = true;
                Projectile.tileCollide = false;
                Projectile.velocity = oldVelocity;
            }
            return false;
        }
        public override bool? CanDamage() => false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //水蜡烛本身也具有范围伤害
        }
        public override bool PreDraw(ref Color lightColor)
        {   
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            //白描边
            for (int i = 0; i < 8;i++)
                SB.Draw(projTex, drawPos + ToRadians(60f * i).ToRotationVector2() * 2f, null, Color.White  with { A = 0 } * Projectile.Opacity, Projectile.rotation + RotFix, ori, Projectile.scale * Projectile.Opacity, 0, 0);
            SB.Draw(projTex, drawPos, null, Color.White * Projectile.Opacity, Projectile.rotation + RotFix, ori, Projectile.scale * Projectile.Opacity, 0, 0);
            //为其绘制一个发光的环。
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            SB.Draw(HJScarletTexture.Texture_BloomShockwave.Value, Projectile.Center - Main.screenPosition, null, Color.DeepSkyBlue * Projectile.Opacity, 0, HJScarletTexture.Texture_BloomShockwave.Origin, 0.13f * DrawGlowScale * Projectile.scale * Projectile.Opacity, SpriteEffects.None, 0);
            SB.Draw(HJScarletTexture.Particle_CrossGlow.Value, Projectile.Center - Main.screenPosition, null, Color.SkyBlue * Projectile.Opacity, 0, HJScarletTexture.Particle_CrossGlow.Origin, 0.20f * DrawGlowScale * Projectile.scale * Projectile.Opacity, SpriteEffects.None, 0);
            SB.End();
            SB.BeginDefault();
            return false;
        }

        private void IgniteFire()
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 dir = (Projectile.rotation).ToRotationVector2();
                Vector2 xOffset = dir.RotatedBy(PiOver2) * Main.rand.NextFloat(-4f, 9f);
                Vector2 spawnPos = Projectile.Center - dir * 12f * i + dir * 12f - xOffset;
                Vector2 fireVel = dir * Main.rand.NextFloat(1.2f, 1.7f);
                new Fire(spawnPos, fireVel, RandLerpColor(Color.SkyBlue, Color.DeepSkyBlue), 40, dir.ToRotation(), 0.8f * Projectile.Opacity, Main.rand.NextFloat(0.08f, 0.10f) * Projectile.Opacity).Spawn();
            }
            //在底下用一圈圆弧粒子将其盘起来
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), DustID.UnusedWhiteBluePurple);
            d.scale *= 1.7f * Projectile.Opacity;
            d.noGravity = true;
        }
    }
}
