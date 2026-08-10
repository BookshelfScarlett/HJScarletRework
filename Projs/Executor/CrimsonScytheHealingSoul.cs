using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class CrimsonScytheHealingSoul : HJScarletProj, IPixelatedRenderer
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public ref float Timer => ref Projectile.ai[0];
        public enum State
        {
            Shoot,
            Homing,
            HomingTarget,
            Fade
        }
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public int HealAmount
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        public int CurSelected
        {
            get => (int)Projectile.localAI[0];
            set => Projectile.localAI[0] = (float)value;
        }
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(24);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.SetupImmnuity(-1);
            Projectile.extraUpdates = 2;
        }
        public static List<Color> BeginColor = new List<Color>()
        {
            Color.HotPink,Color.DeepSkyBlue,Color.LightGray,Color.DarkRed,Color.Purple,Color.DarkGreen
        };
        public static List<Color> EndColor = new List<Color>()
        {
            Color.LightPink,Color.LightSkyBlue,Color.White,Color.Crimson,Color.Violet,Color.LimeGreen
        };
        public override void OnFirstFrame()
        {

            Color c1 = BeginColor[CurSelected];
            Color c2 = EndColor[CurSelected];

            for (int i = 0; i < 16; i++)
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(16), Projectile.velocity.ToRandVelocity(ToRadians(30), 1.2f, 7.4f), RandLerpColor(c1, c2), 45, 1, Main.rand.NextFloat(.85f, 1.15f) * .34f, 0.2f);
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePosEdge(8), Projectile.velocity.ToRandVelocity(0, 1.2f, 7.4f), RandLerpColor(c1, c2), 45, 1, Main.rand.NextFloat(.85f, 1.15f) * .4f);
            }
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
            if (AttackState == State.Shoot)
            {
                Projectile.velocity *= 0.97f;
                Timer++;
                TrailDust();
                if (Timer > Projectile.MaxUpdates * 30f)
                {
                    Projectile.netUpdate = true;
                    AttackState = State.Homing;
                    Timer = 0;
                }
            }
            else if (AttackState == State.Homing)
            {
                float maxTime = 30 * Projectile.MaxUpdates;
                float progress = Utils.GetLerpValue(0, maxTime, Timer, true);
                float lerpSpeed = Lerp(0.1f, 17f, progress);
                float lerpAngle = Lerp(0f, 15f, EaseInCubic(progress));
                Projectile.HomingTarget(Owner.Center, -1, lerpSpeed, 20, lerpAngle);
                //完全确定可以执行转弯了才会播报这些粒子
                //避免棱形粒子在转弯时出戏
                if (progress == 1f)
                    TrailDust();
                if (Projectile.Hitbox.Intersects(Owner.Hitbox))
                {
                    AttackState = State.Fade;
                    Timer = 0;
                    Owner.ScarletHeal(2, Color.LimeGreen);
                }
            }
            else if (AttackState == State.HomingTarget)
            {
                NPC curTar = Main.npc[Projectile.HJScarlet().GlobalTargetIndex];
                if (curTar.IsLegal())
                {
                    float maxTime = 30 * Projectile.MaxUpdates;
                    float progress = Utils.GetLerpValue(0, maxTime, Timer, true);
                    float lerpSpeed = Lerp(0.1f, 17f, progress);
                    float lerpAngle = Lerp(0f, 15f, EaseInCubic(progress));
                    Projectile.HomingTarget(curTar.Center, -1, lerpSpeed, 20, lerpAngle);
                    //完全确定可以执行转弯了才会播报这些粒子
                    //避免棱形粒子在转弯时出戏
                    if (progress == 1f)
                        TrailDust();
                }
            }
            else if (AttackState == State.Fade)
            {

                Projectile.Opacity *= 0.95f;
                Projectile.velocity *= 0.02f;
                if (Projectile.Opacity < 0.08f)
                {
                    Projectile.Kill();
                }
            }
            else
            {

            }
        }
        public void TrailDust()
        {
            if (Projectile.IsOutScreen())
                return;
            Color c1 = BeginColor[CurSelected];
            Color c2 = EndColor[CurSelected];
            if (Main.rand.NextBool(4))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(16), Projectile.velocity / 8f, RandLerpColor(c1, c2), 45, 1, Main.rand.NextFloat(.85f, 1.15f) * .34f, 0.2f);
            if (Main.rand.NextBool(8))
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePosEdge(8), Projectile.velocity / 8f, RandLerpColor(c1, c2), 45, 1, Main.rand.NextFloat(.85f, 1.15f) * .4f);
        }
        public override bool? CanDamage() => AttackState == State.HomingTarget;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackState == State.HomingTarget)
            {
                AttackState = State.Fade;
                Timer = 0;
                Projectile.netUpdate = true;
            }
        }
        public BlendState BlendState => BlendState.Additive;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return;
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D orb = HJScarletTexture.Texture_Spirite.Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float scale = Projectile.scale * 0.40f * Lerp(1f, 1.12f, (float)(Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly)))) * Projectile.Opacity;
            Vector2 newVec = new Vector2(1) * .93f;
            float rot = Main.GlobalTimeWrappedHourly * 1.2f + Projectile.rotation;
            Color firstC = BeginColor[CurSelected];
            Color endC = EndColor[CurSelected];

            SB.Draw(orb, drawPos, null, firstC * 1f, rot, orb.ToOrigin(), scale * newVec, 0, 0);
            orb = HJScarletTexture.Particle_HRShinyOrb.Value;
            SB.Draw(orb, drawPos, null, Color.White * 1f, rot, orb.ToOrigin(), scale * newVec * 0.85f, 0, 0);

            TrailFunc(HJScarletTexture.Trail_ManaStreak.Texture, 1f, firstC * 0.65f);
            TrailFunc(HJScarletTexture.Trail_Lightning0.Texture, 0.8f, endC * 0.95f);
            TrailFunc(HJScarletTexture.Trail_Lightning0.Texture, .5f, Color.White * 0.85f);

            HJScarletMethods.EndShaderAreaPixel();
        }
        public void TrailFunc(Asset<Texture2D> trail, float mult, Color c)
        {
            Effect shader = HJScarletShader.StandardFlowShader;
            float laserLength = 50;
            shader.Parameters["LaserTextureSize"].SetValue(trail.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, trail.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -18.2f);
            shader.Parameters["uColor"].SetValue(c.ToVector4() * Projectile.Opacity * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.15f);
            shader.CurrentTechnique.Passes[0].Apply();
            DrawSetting sets = new(trail.Value);
            List<TrailDrawDate> date = [];
            int length = (int)(Projectile.oldPos.Length * Projectile.Opacity * Clamp(Projectile.velocity.Length(), 0, 1));
            for (int i = 0; i < length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 listPos = Projectile.oldPos[i] + Projectile.Size / 2 + Projectile.SafeDir() * 10f;
                float ratios = i / (float)length;
                date.Add(new(listPos, Color.White, new(0, 40 * mult * Clamp((1 - ratios), 0.32f, 1f)), Projectile.oldRot[i]));
            }
            TrailRender.DrawTrail(date.ToArray(), sets);

        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            return false;
        }
    }
}
