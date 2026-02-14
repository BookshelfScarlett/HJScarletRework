using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Melee
{
    public class CryoblazeHymnProj : ThrownSpearProjClass
    {
        public override string Texture => ProjPath + "Proj_" + nameof(CryoblazeHymn);
        public ref float Timer => ref Projectile.ai[0];
        public enum Style
        {
            Shoot,
            Slowdown
        }
        public Style AttackType
        {
            get => (Style)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public ref float IcePortalTimer => ref Projectile.localAI[0];
        public float Speed = 0f;
        public float SpinMoveTime = 25f;
        public float GeneralProgress = 0f;
        public bool SpawnPortals = false;
        public override void ExSSD()
        {
            Projectile.ToTrailSetting(10, 2);
        }
        public override void ExSD()
        {
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }
        public override void AI()
        {
            if (!Projectile.HJScarlet().FirstFrame)
                Speed = Projectile.velocity.Length();
            AttackAI();
            Projectile.rotation = Projectile.velocity.ToRotation();
            //下面的粒子足够了
            if (HJScarletMethods.OutOffScreen(Projectile.Center))
                return;
            SpawnParticles();
        }
        public void AttackAI()
        {
            if (AttackType == Style.Shoot)
            {
                Timer++;
                if (Timer > 15f)
                {
                    Projectile.netUpdate = true;
                    Timer = 0f;
                    AttackType = Style.Slowdown;
                }
            }
            else
            {
                Timer++;
                if (Timer < SpinMoveTime)
                    Projectile.velocity = Projectile.SafeDir() * Speed * (1f - Timer / (SpinMoveTime));
                else
                    Projectile.Kill();
            }
        }
        public void SpawnParticles()
        {
            GeneralProgress = (1f - Timer / SpinMoveTime);
            GeneralProgress = Clamp(GeneralProgress, 0f, 1f);
            GeneralProgress = AttackType == Style.Shoot ? 1f : GeneralProgress;
            for (int i = 0; i < 2; i++)
            {
                Vector2 dustVel = Projectile.velocity.ToRandVelocity(ToRadians(25f)) * Main.rand.NextFloat(1.2f, 1.6f) * -2.5f;
                //timer会同时总控射弹的速度，这里需要随射弹尽量延展粒子生成来避免过度集中
                float randValue = Main.rand.NextFloat(2f, 30f);
                float ratio = randValue / 30f;
                //查看一下随机值与（1）的大小，距离矛头越近，则更多地采用红色，反之霜火色
                Vector2 extendedPos = randValue * Projectile.SafeDir();
                Vector2 spawnPos = Projectile.Center.ToRandCirclePosEdge(2f) + extendedPos;
                Color fireColor = Color.Lerp(Color.OrangeRed, Color.Orange, 1 - ratio);
                Color smokeColor = Color.Lerp(Color.DeepSkyBlue, Color.White, 1 - ratio);
                //同理, scale也是一样
                float scale = Main.rand.NextFloat(0.10f, 0.23f) * ratio * 1.05f;
                scale = Clamp(scale, 0.10f, scale);
                //不要用add，防止出现可能的过曝
                if (Main.rand.NextBool())
                    new Fire(spawnPos, dustVel, fireColor, 40, RandRotTwoPi, 1f, scale * GeneralProgress).Spawn();
                else
                    new SmokeParticle(spawnPos, dustVel, smokeColor, 40, RandRotTwoPi, 1f, scale * GeneralProgress).Spawn();
            }
            //开始生成两类颜色的shinyorb做点缀
            Vector2 firePos = Projectile.Center.ToRandCirclePos(12f);
            float firedir = RandRotTwoPi;
            Color chooseColor = Main.rand.NextBool() ? RandLerpColor(Color.OrangeRed, Color.Orange) : RandLerpColor(Color.DeepSkyBlue, Color.SkyBlue);
            new TurbulenceShinyOrb(firePos, 0.75f, chooseColor * GeneralProgress, 40, 0.45f * GeneralProgress, firedir).Spawn();
        }

        public override bool PreKill(int timeLeft)
        {
            if (!SpawnPortals)
            {
                QuickSpawn(true, ProjectileType<CryoblazeHymnFirePortal>());
                QuickSpawn(false, ProjectileType<CryoblazeHymnFrostPortal>());
                SoundEngine.PlaySound(SoundID.Item45 with { MaxInstances = 1 },Projectile.Center);
            }
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Vector2 spawnPos = Projectile.Center.ToRandCirclePosEdge(4f) + Projectile.SafeDir() * -60f + Projectile.SafeDir() *i *5f;
                    new TurbulenceShinyOrb(spawnPos, RandZeroToOne, RandLerpColor(Color.SkyBlue, Color.DeepSkyBlue), 30, Main.rand.NextFloat(0.24f, 0.36f), RandRotTwoPi).Spawn();
                    new TurbulenceShinyOrb(spawnPos, RandZeroToOne, RandLerpColor(Color.Orange, Color.OrangeRed), 30, Main.rand.NextFloat(0.24f, 0.36f), RandRotTwoPi).Spawn();
                }
                new SmokeParticle(Projectile.Center.ToRandCirclePos(4f) + Projectile.SafeDir() * -60f + Projectile.SafeDir() * i * 5f, RandVelTwoPi(4f), RandLerpColor(Color.DeepSkyBlue, Color.OrangeRed), 40, RandRotTwoPi, 1, 0.13f).Spawn();
            }
            return true;
        }
        public void QuickSpawn(bool reverse, int type)
        {
            Vector2 spawnPos = Projectile.Center + Projectile.SafeDir() * 40f * reverse.ToDirectionInt();
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), spawnPos, Projectile.SafeDir() * -(18f) * reverse.ToDirectionInt(), type, Projectile.damage, Projectile.knockBack, Owner.whoAmI);
            proj.HJScarlet().GlobalTargetIndex = Projectile.HJScarlet().GlobalTargetIndex;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //不断更新当前射弹所命中的敌对单位。因为这个targetindex会给寒霜弹使用
            target.AddBuff(BuffID.Oiled, GetSeconds(5));
            Projectile.HJScarlet().GlobalTargetIndex = target.whoAmI;
            //立刻跳转第二攻击模式，即减速
            if (AttackType == Style.Shoot)
            {
                AttackType = Style.Slowdown;
                Timer = 0;
                Projectile.netUpdate = true;
                
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public void DrawSideStreak(Color beginColor, Color targetColor, bool NeedPi, float scaleMult)
        {
            int length = 15;
            Texture2D sharpTear = HJScarletTexture.Particle_SharpTear;
            Rectangle cutSource = sharpTear.Bounds;
            cutSource.Width /= 2;
            Vector2 SharpOri = new Vector2(cutSource.Height / 2, cutSource.Width);
            Vector2 dir = Projectile.SafeDir();
            Vector2 offset = dir * 45f;

            for (int i = 0; i < length; i++)
            {
                int magicNumber = i > 9 ? 9 : i;
                if (Projectile.oldPos[magicNumber].Equals(Vector2.Zero))
                    continue;
                float rot = Projectile.rotation - PiOver2 + NeedPi.ToInt() * Pi;
                Vector2 scale = Projectile.scale * new Vector2(0.65f, 2f) * scaleMult;
                float rads = (float)i / length;
                Color drawColor = (Color.Lerp(beginColor, targetColor, rads) with { A = 0 }) * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                Vector2 realPos = Projectile.Center - Main.screenPosition - dir * i * 15f + offset;
                SB.Draw(sharpTear, realPos, cutSource, drawColor * Clamp(Projectile.velocity.Length(), 0, 1), rot, SharpOri, scale, 0, 0);
                SB.Draw(sharpTear, realPos, cutSource, Color.White.ToAddColor() * Clamp(Projectile.velocity.Length(), 0, 1), rot, SharpOri, scale * 0.4f, 0, 0);
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            float RotFix = ToRadians(63.5f);
            DrawSideStreak(Color.DeepSkyBlue, Color.SkyBlue, false, 0.91f * GeneralProgress);
            DrawSideStreak(Color.Orange, Color.OrangeRed, true, 1.12f * GeneralProgress);
            for (int i = 0; i < 8; i++)
                SB.Draw(projTex, drawPos + ToRadians(60f * i).ToRotationVector2() * 1.5f, null, Color.White.ToAddColor(100) * (1 - GeneralProgress), Projectile.rotation + RotFix, ori, 1f, 0, 0);
            for (int i = 0; i < 6; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                float faded = 1 - i / (float)6;
                //平方放缩
                faded = MathF.Pow(faded, 2);
                Color trailColor = Color.WhiteSmoke * faded * 0.8f;
                SB.Draw(projTex, Projectile.oldPos[i] + Projectile.PosToCenter(), null, trailColor * GeneralProgress, Projectile.rotation + RotFix, ori, 1f, 0, 0);
            }
            SB.Draw(projTex, drawPos, null, Color.WhiteSmoke * GeneralProgress, Projectile.rotation + RotFix, ori, 1f, 0, 0);
            return false;
        }
    }
}
