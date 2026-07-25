using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class ASMDExecutionBullet : HJScarletProj
    {
        public override string Texture => GetInstance<ASMDBullet>().Texture;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public enum State
        {
            Shoot,
            Stick
        }
        public NPC CurTarget = null;
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(32);
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 5;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = -1;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void OnFirstFrame()
        {
        }
        public override void ProjAI()
        {
            switch (AttackState)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.Stick:
                    DoStick();
                    break;
            }

        }

        public void DoStick()
        {
            if (CurTarget.IsLegal() && Timer < 11)
            {
                Projectile.velocity *= .01f;
                Projectile.Center = CurTarget.Center;
            }
            else
            {

                for (int i = 0; i < ASMD.ExecutionIceBlockCount; i++)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, RandVelTwoPi(16, 19), ProjectileType<ASMDIceBlock>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    proj.HJScarlet().HasExecutionMechanic = true;
                }

                SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_BoomHit with { Pitch = 0.5f }, Projectile.Center);
                SoundEngine.PlaySound(HJScarletSounds.Sundowner_Fire with { Pitch = 0.5f }, Projectile.Center);
                SoundEngine.PlaySound(HJScarletSounds.Misc_Boom with { Pitch = 0.5f }, Projectile.Center);
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 16f, 20, Projectile.rotation);
                //强行用粉色的烟来把这里的范围覆盖起来
                Vector2 safeDir = Projectile.rotation.ToRotationVector2();
                for (int i = 0; i < 40; i++)
                {
                    for (int j = 0; j < 4; j++)
                        ECSParticle.StarShape(Projectile.Center.ToRandCirclePos(3f), safeDir.RotatedBy(PiOver2 * j + PiOver4) * Main.rand.NextFloat(.1f, 19f), RandLerpColor(Color.RoyalBlue, Color.WhiteSmoke), 30, 1f, 1.091f);
                }
                for (int i = 0; i < 80; i++)
                {
                    if (i % 2 == 0)
                        ECSParticle.HRShinyOrb(Projectile.Center.ToRandCirclePos(3f), RandVelTwoPi(1f, 24f), RandLerpColor(Color.White, Color.RoyalBlue), 30, 1f, Main.rand.NextFloat(.7f, 1.3f) * .1f, 0.45f);
                    ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(5f), RandVelTwoPi(2, 16f), RandLerpColor(Color.White, Color.RoyalBlue), Main.rand.Next(35, 50), 1f, Main.rand.NextFloat(.9f, 1.1f), 0.2f);
                }
                for (int i = 0; i < 60; i++)
                {
                    Color color = RandLerpColor(Color.Lerp(Color.RoyalBlue, Color.CornflowerBlue, 0.50f), Color.LightBlue);
                    Color color2 = RandLerpColor(Color.RoyalBlue, Color.White);
                    new SmokeParticle(Projectile.Center.ToRandCirclePos(5f), RandVelTwoPi(2.7f, 28), color, 40, RandRotTwoPi, .45f, 0.87f * Main.rand.NextFloat(0.8f, 1.1f), false).Spawn();
                    new SmokeParticle(Projectile.Center.ToRandCirclePos(5f), RandVelTwoPi(2.7f, 20), color2, 40, RandRotTwoPi, .45f, 0.77f * Main.rand.NextFloat(0.8f, 1.1f), true).SpawnToPriority();
                    new SmokeParticle(Projectile.Center.ToRandCirclePos(3f), RandVelTwoPi(2.7f, 18), RandLerpColor(Color.DarkGray, Color.DarkOrange), 40, RandRotTwoPi, .40f, 0.78f * Main.rand.NextFloat(0.8f, 1.1f), false).SpawnToPriorityNonPreMult();
                }
                Projectile.Kill();
            }
        }
        public void DoShoot()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(3))
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePos(8), Projectile.velocity / 8f, RandLerpColor(Color.CornflowerBlue, Color.White), 60, 1, Projectile.scale * Main.rand.NextFloat(.9f, 1.1f) * .40f, 6);
            if (Main.rand.NextBool())
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(8), Projectile.SafeDir(), RandLerpColor(Color.SkyBlue, Color.CornflowerBlue), 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .5f, 0.2f);
            ECSParticle.HighResolutionThunder(Projectile.Center.ToRandCirclePos(3), Projectile.SafeDir(), RandLerpColor(Color.CornflowerBlue, Color.White), 40, 1, Projectile.rotation + PiOver2, Projectile.scale * Main.rand.NextFloat(.85f, 1.15f) * .3f, 0);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public void BoomParticle()
        {
            ScarletSound(HJScarletSounds.Frostwave_Boom, Projectile.Center, 0.30f, 1, 0.75f);
            ScarletSound(HJScarletSounds.Frosthammer_SnowCharge, Projectile.Center, 0.30f, 1, -0.75f);
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(15);
                Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(1.2f, 12f);
                ECSParticle.LightntingGlow(pos, vel, RandLerpColor(Color.White, Color.SkyBlue), Main.rand.Next(40, 70), 1f, Main.rand.NextFloat(.75f, 1.1f) * .75f, Main.rand.Next(2, 5));
            }
            for (int i = 0; i < 36; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(3);
                Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(4.2f, 14.8f);
                bool value = Main.rand.NextBool();
                BlendState bs = value ? BlendState.Additive : BlendState.AlphaBlend;
                ECSParticle.SmokeParticle(pos, vel, RandLerpColor(Color.SkyBlue, Color.White), Main.rand.Next(35, 55), RandRotTwoPi, .95f, 0.40f * Main.rand.NextFloat(.7f, 1.1f), value, bs);
            }
            for (int i = 0; i < 24; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(10);
                Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(.42f, 14.8f);
                ECSParticle.SnowCloud(pos, vel, RandLerpColor(Color.White, Color.SkyBlue), Main.rand.Next(35, 55), RandRotTwoPi, .81f, 0.154f * Main.rand.NextFloat(.7f, 1.1f), BlendState.Additive);
            }
            for (int i = 0; i < 18; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(60);
                ECSParticle.TurbulenceShinyOrb(pos, 6.4f, RandLerpColor(Color.White, Color.RoyalBlue), 60, 1, .17f * Main.rand.NextFloat(.85f, 1.1f), glowMult: 0.5f);
            }
            for (int i = 0; i < 44; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(10);
                Vector2 vel = Projectile.Center.GetNormalVector2(pos) * Main.rand.NextFloat(1.2f, 9.8f);
                ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.White, Color.RoyalBlue), 35, 1f, 0.95f * Main.rand.NextFloat(.75f, 1.15f), 0.2f);
            }
            ECSParticle.CrossGlow(Projectile.Center, Color.RoyalBlue, 30, .81f, .48f);
            ECSParticle.CrossGlow(Projectile.Center, Color.White, 30, .81f, .45f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackState == State.Shoot)
            {
                AttackState = State.Stick;
                if (target.IsLegal())
                    CurTarget = target;
                Timer = 0;
                //这样的话会有一个双判的可能性，不过不关我事
                Projectile.extraUpdates = 1;
                Projectile.localNPCHitCooldown = 50;
                Projectile.ResetLocalNPCHitImmunity();
                BoomParticle();
                return;
            }
            if (AttackState == State.Stick)
            {

                float progress = Timer / 6f;
                Projectile.localNPCHitCooldown -= 5;
                SoundEngine.PlaySound(HJScarletSounds.SodomsDisaster_BoomHit with { Pitch = Lerp(0f, 0.4f, progress) }, Projectile.Center);
                Timer += 1;
                ECSParticle.HighResolutionThunder(Projectile.Center.ToRandCirclePos(3), Vector2.Zero, RandLerpColor(Color.CornflowerBlue, Color.White), 40, 1, RandRotTwoPi + PiOver2, Projectile.scale * Main.rand.NextFloat(.85f, 1.15f) * .63f, 2);
                //什么叫你写了这么多就为了处理这个特效爆炸？
                for (int i = 0; i < 30; i++)
                {
                    Vector2 vel = (TwoPi / 45f * i).ToRotationVector2() * Lerp(8f, 15f, progress) * Main.rand.NextFloat(0f, 1f);
                    Vector2 spawnpos = Projectile.Center.ToRandCirclePos(4f) + vel.ToSafeNormalize() * Main.rand.NextFloat() * 2f;
                    Color color = RandLerpColor(Color.Lerp(Color.RoyalBlue, Color.White, 0.50f), Color.White);
                    float scale = 0.40f * Main.rand.NextFloat(0.55f, 1.1f);
                    ECSParticle.SmokeParticle(spawnpos, vel, color, Main.rand.Next(10, 41), RandRotTwoPi, Main.rand.NextFloat(.75f, 1f) * .97f, scale, true, BlendState.Additive);

                    vel = RandVelTwoPi(.4f, 15.5f);
                    color = RandLerpColor(Color.Lerp(Color.RoyalBlue, Color.White, 0.50f), Color.CornflowerBlue);
                    scale = .35f * Main.rand.NextFloat(.65f, 1.1f);
                    ECSParticle.SmokeParticle(spawnpos, vel, color, Main.rand.Next(10, 41), RandRotTwoPi, Main.rand.NextFloat(.75f, 1f) * .97f, scale, true, BlendState.Additive);
                }
                for (int j = 0; j < 10; j++)
                {
                    Vector2 dir = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0f, Lerp(3f, 15.9f, progress));
                    Vector2 pos = Projectile.Center.ToRandCirclePos(3f);
                    ECSParticle.ShinyCrossStarECS(pos, dir, RandLerpColor(Color.WhiteSmoke, Color.CornflowerBlue), Main.rand.Next(15, 50), 1f, 1f * Main.rand.NextFloat(.7f, .9f), .2f);
                }
                for (int i = 0; i < 10; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(2f);
                    Vector2 vel = RandVelTwoPi(.1f, Lerp(1.2f, 11.9f, progress));
                    ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.Lerp(Color.RoyalBlue, Color.CornflowerBlue, .5f), Color.White), Main.rand.Next(15, 50), 1f, .99f * Main.rand.NextFloat(.6f, 1f), .2f);
                }
                for (int i = 0; i < 20; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(2f);
                    Vector2 vel = RandVelTwoPi(.1f, Lerp(1.2f, 14.9f, progress));
                    ECSParticle.HRShinyOrb(pos, vel, RandLerpColor(Color.Lerp(Color.RoyalBlue, Color.White, .5f), Color.CornflowerBlue), Main.rand.Next(15, 50), 1f, .15f * Main.rand.NextFloat(.6f, 1f), .5f);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            float rotFixer = PiOver2;
            float rot = Projectile.rotation + rotFixer;
            for (int i = 0; i < 16; i++)
            {
                SB.Draw(projTex, drawPos + (TwoPi / 16f * i).ToRotationVector2() * 2f, null, Color.RoyalBlue.ToAddColor(), rot, ori, Projectile.scale, 0, 0);
            }
            int length = (int)(Projectile.oldPos.Length * Clamp(Projectile.velocity.Length(), 0, 1));
            for (int i = length - 1; i > 0; i--)
            {
                Vector2 oldPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2;
                float oldrot = Projectile.oldRot[i] + rotFixer;
                float ratios = i / (float)length;
                Color c = Color.Lerp(Color.MediumAquamarine, Color.Transparent, ratios);
                float opac = Lerp(.65f, 0.45f, ratios) * Clamp(Projectile.velocity.Length(), 0, 1);
                float oldScale = Lerp(Projectile.scale * .55f, Projectile.scale * .10f, ratios);
                c *= opac;
                for (int j = 0; j < 4; j++)
                {
                    SB.Draw(projTex, oldPos + (TwoPi / 4f * j).ToRotationVector2() * 2, null, c.ToAddColor(), oldrot, ori, oldScale, 0, 0);
                }
                SB.Draw(projTex, oldPos, null, c, rot, ori, oldScale, 0, 0);
            }
            SB.EnterShaderArea(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.Blue, 0.3265f, 0.95f);
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.RoyalBlue, 1.5f, 0.95f);
            DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.CornflowerBlue, 1.25f, .95f, 1.5f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.White, 0.315f, .695f, 1.25f);
            SB.EndShaderArea();
            if(AttackState == State.Stick)
            {
                drawPos += Main.rand.NextVector2Circular(3, 3);
            }
            SB.Draw(projTex, drawPos, null, Color.White, rot, ori, Projectile.scale, 0, 0);
            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return;

            if (Projectile.oldPos.Length < 3)
                return;
                float rad = 1;
            if (Projectile.timeLeft < 50)
                rad = Projectile.timeLeft / 50f * Projectile.Opacity * Clamp(Projectile.velocity.Length(), 0, 1);
            int posCount = (int)((Projectile.oldPos.Length) * rad);
            if (posCount < 2)
                return;
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(useTex.Width(), useTex.Height()));
            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 210f * offsetHeight);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * Projectile.Opacity * alphaValue * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.06f);
            shader.CurrentTechnique.Passes[0].Apply();

            DrawSetting drawSetting = new(useTex.Value);
            List<TrailDrawDate> trailDrawDates = [];
            

            for (int j = 0; j < posCount; j++)
            {
                if (Projectile.oldPos[j] != Vector2.Zero)
                {
                    Vector2 vec = Projectile.oldRot[j].ToRotationVector2().RotatedBy(PiOver2);
                    Vector2 drawPos = Projectile.oldPos[j] + Projectile.Size / 2 + vec * -1.2f;
                    trailDrawDates.Add(new(drawPos, drawColor, new Vector2(0, 35 * multipleSize * Projectile.scale), Projectile.oldRot[j]));
                }
            }
            TrailRender.RenderTrail([.. trailDrawDates], drawSetting);
        }
    }
}
