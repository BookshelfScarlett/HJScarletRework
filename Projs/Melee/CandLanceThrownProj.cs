using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class CandLanceThrownProj : ThrownSpearProjClass
    {
        public override string Texture => GetInstance<Candlance>().Texture;
        public override void SetStaticDefaults() => Projectile.ToTrailSetting();
        public float Timer = 0;
        public enum Style
        {
            Shooted,
            Hit
        }
        public bool DonRiseLamp = false;
        public Style AttackType
        {
            get => (Style)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public ref float ExtraTimer => ref Projectile.ai[1];
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 16;
            Projectile.noEnchantmentVisuals = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 2;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            switch(AttackType)
            {
                case Style.Shooted:
                    DoShooted();
                    break;
                case Style.Hit:
                    DoHit();
                    break;
            }
            Vector2 speedOffset = Projectile.velocity / 4;
            Vector2 dir = Projectile.SafeDir();
            Vector2 mountedPos = Projectile.Center + dir * 60f;
            //总体在底下绘制一些别的粒子，这里用的是树叶
            ExtraTimer += 0.42f;
            for (int i = 0; i < 4; i++)
            {
                Vector2 spawnPos = mountedPos + dir.RotatedBy(PiOver2) * MathF.Sin(ExtraTimer - i * 0.1f) * (9.0f);
                new ShinyOrbParticle(spawnPos - speedOffset * i, dir * 1.2f, RandLerpColor(Color.DeepSkyBlue,Color.LightBlue), 25, 0.4f).Spawn();
                Dust shinyDust = Dust.NewDustPerfect(mountedPos + Main.rand.NextVector2Circular(8f, 8f), DustID.UnusedWhiteBluePurple);
                shinyDust.scale *= Main.rand.NextFloat(1.1f, 1.2f);
                shinyDust.velocity = dir * 1.2f;
            }

            
        }
        private void DoShooted()
        {
            Timer = 1;    

        }

        private void DoHit()
        {
            //在AI这里向上投射火焰，方便一些同步问题
            if (Timer > 0f && !DonRiseLamp)
            {
                //这里需要遍历一遍所取位置是否处于wall里面。如果是则回退直到适合为止
                Vector2 projDir = Projectile.SafeDirByRot();
                Vector2 spawnPos = Projectile.Center;
                Vector2 possibleLampPos = new(Projectile.Center.X - 30, Projectile.Center.Y - 30);
                bool ifHitWall = Collision.SolidCollision(possibleLampPos, 60, 60);
                if (ifHitWall)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        Vector2 fixedPos = possibleLampPos - projDir * i * 10f;
                        //魂火灯本身的体积大小
                        bool isStillHitWall = Collision.SolidCollision(fixedPos, 60, 60);
                        //如果当前的位置已经合适，将spawnPos重设
                        if (!isStillHitWall)
                        {
                            spawnPos = fixedPos;
                            break;
                        }
                    }
                }
                Timer = -1;
                Projectile fireLight = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(ToRadians(-15f), ToRadians(15f))) * Main.rand.NextFloat(14f, 18f), ProjectileType<CandLanceFire>(), Projectile.damage, 0f, Owner.whoAmI);
                fireLight.timeLeft = Main.rand.Next(100, 150);
                fireLight.HJScarlet().GlobalTargetIndex = Projectile.HJScarlet().GlobalTargetIndex;
                fireLight.ai[2] = Main.rand.NextFloat(-10f, 10f);
            }
            //是的孩子们，这是他妈的硬编码
            Projectile.damage *= 0;
            Projectile.Opacity -= 0.02f;
            Projectile.velocity *= 0.94f;
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Vector2 mountedPos = Projectile.Center + dir * 80f - 4f  * dir.RotatedBy(PiOver2);
            for (int i = 0; i < 2; i++)
            {
                new Fire(mountedPos - dir * 60f * i + Main.rand.NextVector2Circular(8f, 6f), Vector2.Zero, RandLerpColor(Color.SkyBlue, Color.Blue), 40, dir.ToRotation(), 1f, 0.1f).Spawn();
            }
            if (Projectile.Opacity <= 0f)
                Projectile.Kill();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (AttackType == Style.Shooted)
            {
                //撞墙需要直接处死不要燃起魂火灯。
                AttackType = Style.Hit;
                Projectile.tileCollide = false;
                DonRiseLamp = true;
                Projectile.velocity = oldVelocity;
            }
            return false;
        }
        public override bool PreKill(int timeLeft)
        {
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //本地跑切割特效
            for (int i = 0; i < 13; i++)
                new StarShape(Projectile.Center, Projectile.velocity.ToRandVelocity(ToRadians(10f), 3f, 7f), RandLerpColor(Color.SkyBlue, Color.DeepSkyBlue), Main.rand.NextFloat(0.8f, 1.3f), 30).Spawn();
            //不用考虑特判倒是
            AttackType = Style.Hit;
            Projectile.HJScarlet().GlobalTargetIndex = target.whoAmI;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawProjItSelf();
            //为其绘制一个发光的环，给矛尖的火用
            DrawFireGlow(); 
            return false;
        }

        private void DrawProjItSelf()
        {
            Projectile.DrawGlowEdge(Color.White * Projectile.Opacity, rotFix: ToRadians(135));
            Projectile.DrawProj(Color.White * Projectile.Opacity, 4, 0.7f, rotFix: ToRadians(135));
        }

        public void DrawFireGlow()
        {
            Texture2D glow = HJScarletTexture.Particle_CrossGlow.Value;
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Vector2 drawPos = Projectile.Center - Main.screenPosition + Projectile.SafeDir() * 80f;
            Vector2 offset = Projectile.SafeDir().RotatedBy(PiOver2) * 3f;
            SB.Draw(glow, drawPos + offset, null, Color.DeepSkyBlue * Projectile.Opacity, Projectile.rotation, glow.Size() / 2, 0.25f * Projectile.scale * Projectile.Opacity, SpriteEffects.None, 0);
            SB.Draw(glow, drawPos + offset, null, Color.AliceBlue * Projectile.Opacity, Projectile.rotation, glow.Size() / 2, 0.20f * Projectile.scale * Projectile.Opacity, SpriteEffects.None, 0);
            SB.End();
            SB.BeginDefault();
        }
    }
}
