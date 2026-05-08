using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class DeepToneMouth : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public enum State
        {
            Shoot,
            Lock,
            BounceOut
        }
        public NPC CurTarget = null;
        public Vector2 StoredPosition;
        public Vector2 BouceToPos;
        public float RandValue = 0;
        public bool ScaleUp = false;
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[0];
        public AnimationStruct Helper = new(2);
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(20, 2);
        }
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 16;
            Projectile.extraUpdates = 2;
            Projectile.penetrate = -1;
            Projectile.SetupImmnuity(60);
            Projectile.scale = 0.70f;
            Projectile.tileCollide = true;
            Projectile.Opacity = 0;
            Projectile.timeLeft = GetSeconds(5);
            Projectile.noEnchantmentVisuals = true;
        }
        public float HideCounter = 0;
        public override void OnFirstFrame()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            RandValue = Main.rand.NextFloat(1f, 6f);
            Helper.MaxProgress[0] = 15;
            Helper.MaxProgress[1] = 15;
        }
        public override void ProjAI()
        {
            UpdateAttackAI();
            UpdateParticle();
        }

        public void UpdateAttackAI()
        {
            switch (AttackState)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.Lock:
                    DoLock();
                    break;
                case State.BounceOut:
                    DoBounceOut();
                    break;
            }
        }

        public void DoBounceOut()
        {
            Projectile.Kill();
            return;

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.BounceOnTile(oldVelocity);
            return false;
        }
        public void DoLock()
        {
            if (!CurTarget.IsLegal() || Projectile.timeLeft < GetSeconds(1))
            {
                AttackState = State.BounceOut;
                Projectile.netUpdate = true;
                Timer = 0;
                //逆向速度，我们准备让速度提起来了
                Projectile.velocity *= -1;
                Projectile.velocity = Projectile.SafeDir() * 1f;
                BouceToPos = Projectile.Center + Projectile.SafeDir() * 150f;
                Projectile.scale = 1;
                return;
            }
            Projectile.Center = CurTarget.Center + StoredPosition;
            if (ScaleUp)
            {
                Projectile.scale = Lerp(Projectile.scale, 0.82f, 0.23f);
                if (Projectile.scale >= 0.81f)
                {
                    Projectile.scale = .82f;
                    ScaleUp = false;
                }
            }
            else
                Projectile.scale = Lerp(Projectile.scale, .7f, 0.23f);
        }

        public void DoShoot()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
            if (Projectile.MeetMaxUpdatesFrame(Timer, RandValue))
            {
                if (Projectile.GetTargetSafe(out NPC target, true, 1100))
                {
                    Projectile.Opacity = Lerp(Projectile.Opacity, 1f, .2f);
                    CurTarget = target;
                }
                else
                {
                    Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0.1f);
                    if (Projectile.Opacity <= 0.02f)
                    {
                        Projectile.Opacity = 0;
                        Projectile.velocity *= 0;
                    }

                }
                if (CurTarget.IsLegal() && Projectile.Opacity > .98f)
                {
                    if (HideCounter > 0.92f)
                    {
                        Projectile.timeLeft = GetSeconds(3);
                        Projectile.Opacity = 1;
                        HideCounter = 0;
                        Vector2 vel = (CurTarget.Center - Projectile.Center).ToSafeNormalize() * 18f;
                        Projectile.velocity = vel;
                        for (int i = 0; i < 16; i++)
                        {
                            Vector2 dir = Vector2.UnitX.RotatedBy((ToRadians(360f / 16 * i)));
                            Vector2 pos = Projectile.Center + dir * 12f;
                            new SmokeParticle(pos, dir * Main.rand.NextFloat(2.2f, 2.6f), RandLerpColor(Color.DarkSeaGreen, Color.Lerp(Color.Black, Color.DarkGreen, 0.75f)), 30, RandRotTwoPi, 1, Main.rand.NextFloat(0.2f, 0.4f) * 0.67f, true).SpawnToPriorityNonPreMult();
                        }
                        for (int i = 0; i < 16; i++)
                        {
                            Vector2 dir = Vector2.UnitX.RotatedBy((ToRadians(360f / 16 * i)));
                            Vector2 pos = Projectile.Center + dir * 2f;
                            new SmokeParticle(pos, dir * Main.rand.NextFloat(0.22f, 2.6f), RandLerpColor(Color.DarkSeaGreen, Color.Lerp(Color.Black, Color.DarkGreen, 0.75f)), 30, RandRotTwoPi, 1, Main.rand.NextFloat(0.2f, 0.4f) * 0.8f, true).SpawnToPriorityNonPreMult();
                        }
                        for (int i = 0; i < 16; i++)
                        {
                            Vector2 dir = Vector2.UnitX.RotatedBy((ToRadians(360f / 16 * i)));
                            Vector2 pos = Projectile.Center + dir * 2f;
                            new ShinyCrossStar(Projectile.Center.ToRandCirclePos(16f), vel.ToRandVelocity(ToRadians(20f), 0.2f, 6.3f), RandLerpColor(Color.DarkSeaGreen, Color.DarkGreen), 40, 0, 1, 0.8f, false).Spawn();
                        }

                        new CrossGlow(Projectile.Center, Color.DarkSeaGreen, 40, 1, 0.15f).Spawn();
                        new CrossGlow(Projectile.Center, Color.SeaGreen, 40, 1, 0.12f).Spawn();
                        new CrossGlow(Projectile.Center, Color.White, 40, 1, 0.09f).Spawn();
                    }
                    Projectile.HomingTarget(CurTarget.Center, -1, 12f, 20f);
                }
                else
                {
                    HideCounter = Lerp(HideCounter, 1.01f, 0.1f);
                    if (Main.rand.NextFloat() < HideCounter && HideCounter > .98f)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            Vector2 pos = Projectile.Center.ToRandCirclePos(30f);
                            Vector2 dir = (pos - Projectile.Center).ToSafeNormalize();
                            new SmokeParticle(pos, dir * Main.rand.NextFloat(0f, 1.6f), RandLerpColor(Color.DarkOliveGreen, Color.Lerp(Color.Black, Color.DarkGreen, 0.45f)), 30, RandRotTwoPi, 1, Main.rand.NextFloat(0.2f, 0.4f) * 0.87f, true).SpawnToPriorityNonPreMult();
                        }
                    }
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity.ToSafeNormalize() * 0.0f, 0.05f);
                }
            }
            else
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 1f, 0.3f);
                if (Projectile.Opacity > 0.98f)
                    Projectile.Opacity = 1;
            }
        }

        public void UpdateParticle()
        {
            if (AttackState == State.Lock && !Projectile.FinalUpdateNextBool(4))
            {
                return;

            }
            if (Projectile.IsOutScreen() || Main.rand.NextFloat() < HideCounter)
                return;
            if (Projectile.FinalUpdateNextBool(5))
                new SmokeParticle(Projectile.Center.ToRandCirclePos(10f), Projectile.velocity / 8f, RandLerpColor(Color.DarkOliveGreen, Color.DarkSeaGreen), 30, RandRotTwoPi, 1, Main.rand.NextFloat(0.2f, 0.4f) * 0.35f, true).Spawn();
            if (Main.rand.NextBool(4))
                new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(10f), Projectile.velocity / 8f, RandLerpColor(Color.DarkGreen, Color.DarkOliveGreen), 30, 0, 1, 0.46f * Main.rand.NextFloat(0.5f, 0.8f), false).Spawn();

        }

        public override bool? CanHitNPC(NPC target)
        {
            switch (AttackState)
            {
                case State.Shoot:
                    if (Projectile.MeetMaxUpdatesFrame(Timer, 2))
                        return null;
                    break;
                case State.Lock:
                    if (CurTarget is not null && target.Equals(CurTarget))
                        return null;
                    break;
                case State.BounceOut:

                    break;
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackState == State.Shoot)
            {
                if (target.IsLegal())
                {
                    CurTarget = target;
                    Timer = 0;
                    AttackState = State.Lock;
                    StoredPosition = Projectile.Center - target.Center;
                    Projectile.timeLeft = 400;
                    //不要直接设置到最底层的速度
                    Projectile.velocity *= 0.01f;
                }
                else
                    Projectile.timeLeft = 2;
            }
            if (AttackState == State.Lock)
            {
                ScaleUp = true;
                if (Main.rand.NextBool(7))
                {
                    Vector2 vel = Projectile.SafeDir() * 12f;
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, vel, ProjectileType<DeepToneHealOrb>(), 0, 0, Owner.whoAmI);
                    ((DeepToneHealOrb)proj.ModProjectile).UseHeal = true;
                    proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
                    for (int i = 0; i < 16; i++)
                    {
                        new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(10f), vel.ToRandVelocity(ToRadians(5f), 0.4f, 3.9f), RandLerpColor(Color.DarkGreen, Color.DarkOliveGreen), 30, 0, 1, 0.76f * Main.rand.NextFloat(0.5f, 0.8f), false).Spawn();

                    }
                }
            }
        }
        public void DrawTrail()
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            Texture2D tex = Projectile.GetTexture();
            Vector2 orig = tex.Size() / 2;
            int drawLength = Projectile.oldPos.Length / 2;
            for (int i = drawLength - 1; i >= 0; i--)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 trailingDrawPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.10f) + Projectile.PosToCenter();
                float faded = 1 - i / (float)drawLength;
                //平方放缩
                faded = MathF.Pow(faded, 3);
                Color trailColor = Color.Lerp(Color.DarkSeaGreen, Color.Lerp(Color.Lime, Color.White, 0.8f), faded) * 0.9f * Projectile.Opacity;
                float opa = Lerp(0.65f, 1f, faded);
                trailColor = trailColor.ToAddColor((byte)(Lerp(150, 0, faded))) * opa;
                float scaleMult = Lerp(0.40f, 1f, faded);
                SB.Draw(tex, trailingDrawPos, null, trailColor * Projectile.Opacity, Projectile.oldRot[i] + PiOver4, orig, Projectile.scale * scaleMult, 0, 0);
            }
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Trail_ManaMegaBeam.Texture, Color.DarkSeaGreen, 0.5f, 0.8f);
            DrawTrails(HJScarletTexture.Trail_ManaStreakTiny.Texture, Color.DarkOliveGreen, 0.8f, 0.80f);
            DrawTrails(HJScarletTexture.Trail_ManaStreak.Texture, Color.DarkSeaGreen, 0.5f, 0.85f);
            SB.EndShaderArea();

        }
        public override bool PreDraw(ref Color lightColor)
        {
            float rotFixer = PiOver4;
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            if (AttackState != State.BounceOut)
                DrawTrail();
            Projectile.DrawGlowEdge(Color.White * Projectile.Opacity, rotFix: rotFixer);
            Projectile.DrawProj(Color.White * Projectile.Opacity, 1, rotFix: rotFixer);
            Texture2D ring = HJScarletTexture.Particle_RingShiny.Value;
            Texture2D star = HJScarletTexture.Particle_KiraStarGlow.Value;
            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return;
            if (Projectile.oldPos.Length < 18)
                return;
            Effect shader = HJScarletShader.StandardFlowShader;
            float laserLength = 50;
            shader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -10.2f);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue * Projectile.Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.1f);
            shader.CurrentTechnique.Passes[0].Apply();
            GD.Textures[1] = HJScarletTexture.Texture_Spirite.Value;
            GD.SamplerStates[1] = SamplerState.PointWrap;

            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> _, Projectile.oldPos, Projectile.oldRot);
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = validPosition.Count;
            for (int j = 0; j < posCount - 1; j++)
            {
                float rot = (validPosition[j + 1] - validPosition[j]).ToRotation();
                float ratio = j / (posCount - 1);
                Vector2 posOffset = rot.ToRotationVector2().RotatedBy(PiOver2) * offsetHeight;
                trailDrawDates.Add(new(validPosition[j] + Projectile.Size / 2, drawColor, new Vector2(0, 22 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }
    }
}
