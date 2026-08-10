using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Assistance;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class GhostKnifeProj : HJScarletProj
    {
        public override string Texture => GetInstance<GhostKnife>().Texture;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(12);
        }
        public ref float AngleTimer => ref Projectile.ai[2];
        public bool SetHoming
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value ? 1 : 0;
        }

        public override void ExSD()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.extraUpdates = 3;
            Projectile.SetupImmnuity(30);
            Projectile.penetrate = 7;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void OnFirstFrame()
        {
            if (!SetHoming)
                ScarletSound(HJScarletSounds.Misc_KnifeTossAlt, Projectile.Center, 0.45f, variantType: 2, pitch: .4f, instances: 0, pitchVariance: 0.15f);
            else
                ScarletSound(HJScarletSounds.Misc_KnifeTossAlt, Projectile.Center, 0.45f, variantType: 2, pitch: .4f, instances: 0, pitchVariance: 0.15f);
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 offset = Projectile.SafeDir() * 42f;
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(3) && !SetHoming)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(8) - offset;
                Vector2 vel = RandVelTwoPi(0.5f, 1.2f) + Projectile.velocity / 8f;
                float scale = Main.rand.NextFloat(.8f, 1.15f) * Projectile.scale * .13f;
                ECSParticle.TurbulenceShinyOrb(pos + vel, .3f, RandLerpColor(Color.SkyBlue, Color.White), 50, 1, scale, RandRotTwoPi, 0.2f);
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(8) - offset;
                Vector2 vel = RandVelTwoPi(0.5f, 1.2f) + Projectile.velocity / 8f;
                float scale = Main.rand.NextFloat(.8f, 1.15f) * Projectile.scale * .03f;
                ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.White, Color.RoyalBlue), 45, 1, 0.64f, .2f);
            }

        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SetGeneralParticle(Projectile.oldPosition);
            if (!Owner.HasProj<GhostKnifeMark>())
                Projectile.AddExecutionTimeImmediate<GhostKnife>();
            if (!SetHoming)
            {
                ScarletSound(SoundID.Item109, Projectile.Center, volume: .65f, pitch: 0.6f, pitchVariance: 0.3f);
                Vector2 beginPos = target.Center + Vector2.UnitX.RotatedByRandom(TwoPi).ToSafeNormalize() * Main.rand.NextFloat(150f, 170f);
                Vector2 dir = beginPos.GetNormalVector2(target.Center) * 16f * 1.15f;
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), beginPos, dir, ProjectileType<GhostKnifePhantom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                SetGeneralParticle(proj.position);
            }
            else
                AngleTimer = -5f * Projectile.MaxUpdates * 1;
        }
        public void SetGeneralParticle(Vector2 pos1)
        {
            Vector2 pos = pos1 + Projectile.Size / 2;
            for (int i = 0; i < 8; i++)
            {
                ECSParticle.ShinyCrossStarECS(pos, RandVelTwoPi(1.2f, 2.2f), Color.White, 40, 1, 0.4f);
            }
            for (int i = 0; i < 6; i++)
            {
                ECSParticle.SmokeParticle(pos, RandVelTwoPi(1.2f, 3.2f), RandLerpColor(Color.White, Color.LightSkyBlue), 40, 1, 1, 0.21f, blendstate: BlendState.Additive);
            }
            ECSParticle.StarShape(pos, Projectile.oldVelocity.ToSafeNormalize() * .01f, Color.LightBlue, 40, 1, 0.94f);
            ECSParticle.StarShape(pos, Projectile.oldVelocity.RotatedBy(PiOver2).ToSafeNormalize() * .01f, Color.LightBlue, 40, 1, 0.94f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 offst = Projectile.SafeDir() * 22f;
            DrawProj(offst);
            SB.EnterShaderArea();
            DrawTrail(HJScarletTexture.Trail_ManaStreak.Texture, 1f, Color.DeepSkyBlue * 0.65f);
            DrawTrail(HJScarletTexture.Trail_Lightning0.Texture, 0.8f, Color.LightSkyBlue * 0.95f);
            DrawTrail(HJScarletTexture.Trail_Lightning0.Texture, .5f, Color.White * 0.85f);

            SB.EndShaderArea();
            return false;
        }
        public void DrawProj(Vector2 offset)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            drawPos -= offset;
            Texture2D tex = Projectile.GetTexture();
            Vector2 orig = tex.Size() / 2;
            float disapperRatios = 1;
            int length = (int)((Projectile.oldPos.Length - 4) * (disapperRatios));
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = EaseInOutExpo(1 - i / (float)length);
                Vector2 pos = Projectile.oldPos[i] + Projectile.PosToCenter() - offset;
                float rot = Projectile.oldRot[i];
                int addLerp = (int)(Lerp(0, 105, ratios));
                Color c = Color.Lerp(Color.White, Color.WhiteSmoke, ratios).ToAddColor((byte)addLerp) * (Lerp(0.14f, 1f, ratios));
                float scale = Projectile.scale * Lerp(.35f, 1f, ratios);
                SB.Draw(projTex, pos, null, c, rot + PiOver4, orig, scale, 0, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.Draw(projTex, drawPos + (TwoPi / 8 * i).ToRotationVector2() * 1.5f, null, Color.WhiteSmoke.ToAddColor(), Projectile.rotation + PiOver4, ori, Projectile.scale, 0, 0);
            Color mainC = Color.Lerp(Color.White, Color.WhiteSmoke, disapperRatios);
            SB.Draw(projTex, drawPos, null, mainC, Projectile.rotation + PiOver4, ori, Projectile.scale, 0, 0);
        }
        public void DrawTrail(Asset<Texture2D> trail, float multValue, Color color)
        {
            Effect shader = HJScarletShader.StandardFlowShader;
            float laserLength = 50;
            shader.Parameters["LaserTextureSize"].SetValue(trail.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, trail.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -45.2f);
            shader.Parameters["uColor"].SetValue(color.ToVector4() * Projectile.Opacity * Clamp(Projectile.velocity.Length(), 0f, 1f));
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.15f);
            shader.CurrentTechnique.Passes[0].Apply();
            DrawSetting sets = new(trail.Value);
            List<TrailDrawDate> date = [];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 listPos = Projectile.oldPos[i] + Projectile.Size / 2 - Projectile.SafeDir() * 25f;
                float ratios = i / (float)Projectile.oldPos.Length;
                date.Add(new(listPos, Color.White, new(0, 30 * multValue * Clamp((1 - ratios), 0.72f, 1f)), Projectile.oldRot[i]));
            }
            TrailRender.DrawTrail(date.ToArray(), sets);
        }
    }
}
