using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.IDSets;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class CrimsonScytheSoulStone : HJScarletProj, IPixelatedRenderer
    {
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public enum State
        {
            Shoot,
            Idle,
            Explosion
        }
        public ref float Timer => ref Projectile.ai[0];
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public AnimationStruct Helper = new AnimationStruct(3);
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16);
            ScarletProjIDSets.DivingProjectile[Type] = true;
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 84;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.SetupImmnuity(-1);
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public List<Vector2> CenterPosList = [];
        private Vector2 TopLeftPoint = new Vector2(0, 0);
        private Vector2 TopRightPoint = new Vector2(30, -100);
        private Vector2 BottomLeftPoint = new Vector2(30, 100);
        private Vector2 BottomRightPoint = new Vector2(0, 0);
        private Vector4 RandValueSummary = new Vector4();
        public float RandOmega = 0;
        public override void OnFirstFrame()
        {
            RandValueSummary = new Vector4(Main.rand.NextFloat(.7f, 1.1f), Main.rand.NextFloat(.7f, 1.1f), Main.rand.NextFloat(.7f, 1.1f), Main.rand.NextFloat(0.7f, 1.1f));
            float maxPoints = 60;
            for (int i = 0; i < maxPoints; i++)
            {
                float progress = i / maxPoints;
                Vector2 finalPos = Vector2.CatmullRom(TopLeftPoint, TopRightPoint, BottomLeftPoint, BottomRightPoint, progress);
                finalPos.X *= 0.9f;
                finalPos.Y *= 0.18f;
                CenterPosList.Add(finalPos.RotatedBy(PiOver2));
            }
            RandOmega = RandRotTwoPi;
        }
        public override void ProjAI()
        {
            switch (AttackState)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.Idle:
                    DoIdle();
                    break;
                case State.Explosion:
                    DoExplosion();
                    break;
            }
            Projectile.timeLeft = 2;
        }

        public void DoShoot()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= .96f;
            if (Projectile.velocity.LengthSquared() < 0.1f * 0.1f)
            {
                AttackState = State.Idle;
                Projectile.netUpdate = true;
            }

        }
        public void DoIdle()
        {
            Vector2 vel = (float)Math.Sin(Main.GlobalTimeWrappedHourly / 1f + ToDegrees(RandRotTwoPi)) * 0.21f * Vector2.UnitY;
            bool anti = Projectile.MinionAntiClump(0.5f / Projectile.MaxUpdates);
            if (!anti)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, vel, 0.12f);
            if (Main.rand.NextBool(20))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(10f, 20f), RandDirTwoPi, Color.White, 40, 1, 0.24f * Main.rand.NextFloat(0.8f, 1.1f));
            if (Main.rand.NextBool(18))
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(15), 1.1f * Main.rand.NextFloat(0.8f, 1.1f), Color.White, 60, 1, 0.08f * Main.rand.NextFloat(0.8f, 1.1f));
            if (Main.rand.NextBool(16))
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(10f, 20f), RandDirTwoPi, RandLerpColor(Color.Gray, Color.White), 40, RandRotTwoPi, 1, 0.24f * Main.rand.NextFloat(0.75f, 1.1f), Main.rand.NextBool(), BlendState.Additive);
        }
        public void DoExplosion()
        {
            ScarletSound(HJScarletSounds.Tlipoca_StoneShatter, Projectile.Center, 0.23f, 1, -0.4f);
            Projectile.Kill();
        }
        public override void OnKill(int timeLeft)
        {
            Vector2 pos = Projectile.Center;
            int curSelected = Main.rand.Next(0, CrimsonScytheHealingSoul.BeginColor.Count);
            Color c1 = CrimsonScytheHealingSoul.BeginColor[curSelected];
            Color c2 = CrimsonScytheHealingSoul.EndColor[curSelected];
            ECSParticle.CrossGlow(pos, c2, 45, 1, 0.394f, 0.2f);
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.TurbulenceShinyOrb(pos.ToRandCirclePosEdge(4), Main.rand.NextFloat(0.8f, 1.15f) * 3.4f, RandLerpColor(c1, c2), 140, 1, .1f);
            }
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.ShinyCrossStarECS(pos, RandVelTwoPi(1.2f, 3.3f), RandLerpColor(c1, c2), 120, 1, .71f);
            }

            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, RandVelTwoPi(10f, 15f), ProjectileType<CrimsonScytheHealingSoul>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
            proj.localAI[0] = curSelected;
            proj.localAI[1] = Projectile.localAI[0];
            proj.originalDamage = Projectile.damage;
            if (Owner.HJScarlet().crimsonScytheAttackCounter > 0 && Projectile.HJScarlet().GlobalTargetIndex != -1)
            {
                proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, RandVelTwoPi(10f, 15f), ProjectileType<CrimsonScytheHealingSoul>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                ((CrimsonScytheHealingSoul)proj.ModProjectile).AttackState = CrimsonScytheHealingSoul.State.HomingTarget;
                proj.localAI[0] = curSelected;
                proj.localAI[1] = Projectile.localAI[0];
                proj.HJScarlet().GlobalTargetIndex = Projectile.HJScarlet().GlobalTargetIndex;
                proj.originalDamage = Projectile.damage;
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            DrawGlow();
            DrawFlowTrail();
            DrawStone();
            return false;
        }
        public void DrawFlowTrail()
        {
            SB.EnterShaderArea();
            Texture2D glow = HJScarletTexture.Particle_OpticalLineGlow.Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float scale = Projectile.scale * 0.175f;
            SB.Draw(glow, drawPos, null, Color.White * .65f, 0, glow.Size() / 2, scale * .95f * new Vector2(0.41f, 1f), SpriteEffects.FlipHorizontally, 0);
            SB.EndShaderArea();

        }
        public void TrailFunc(Texture2D tex, Color c, float mult)
        {
            List<ScarletVertex> vertices = new List<ScarletVertex>();
            Vector2 dir = (-Vector2.UnitY) * 12f * mult;
            for (int i = 0; i < CenterPosList.Count; i++)
            {
                float progress = (float)i / CenterPosList.Count;
                Vector2 pos = Projectile.Center - Main.screenPosition - Vector2.UnitY * 15 - Vector2.UnitX;
                Vector2 posHead = CenterPosList[i] + pos;
                Vector2 posSrc = CenterPosList[i] + pos + dir * Projectile.Opacity;
                vertices.Add(new ScarletVertex(posHead, c, new Vector3(progress, 0, 0)));
                vertices.Add(new ScarletVertex(posSrc, c, new Vector3(progress, 1, 0)));
            }
            if (vertices.Count < 3)
                return;
            GD.Textures[0] = tex;
            GD.SamplerStates[0] = SamplerState.PointWrap;
            GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
        }
        public void DrawGlow()
        {
            SB.EnterShaderArea();
            Texture2D glow = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float scale = Projectile.scale * 1.73f;
            SB.Draw(glow, drawPos, null, Color.White * .55f, 0, glow.Size() / 2, scale * .95f, SpriteEffects.FlipHorizontally, 0);
            SB.EndShaderArea();
        }

        public void DrawStone()
        {
            float velClamp = Clamp(Projectile.velocity.Length(), 0, 1);
            int length = (int)(Projectile.oldPos.Length);
            Texture2D tex = Projectile.GetTexture();
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = i / (float)length;
                Vector2 oldPos = Projectile.oldPos[i] + Projectile.PosToCenter();
                float rot = Projectile.oldRot[i];
                Color beginColor = Color.White;
                Color targetColor = Color.Lerp(Color.White, Color.Silver, 0.68f);
                Color finalColor = Color.Lerp(beginColor, targetColor, ratios) with { A = 150 };
                float opa = EaseOutCubic(ratios);
                float scale = Projectile.scale * (Lerp(1f, 0.85f, ratios));
                SB.Draw(tex, oldPos, null, finalColor * (1 - opa), 0, tex.Size() / 2, scale, 0, 0);
            }
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            for (int i = 0; i < 8; i++)
                SB.Draw(tex, drawPos + (TwoPi / 8f * i).ToRotationVector2() * 1.15f, null, Color.White.ToAddColor(), 0, tex.Size() / 2, Projectile.scale, 0, 0);
            SB.Draw(tex, drawPos, null, Color.LightGray.ToAddColor(215), 0, tex.Size() / 2, Projectile.scale, 0, 0);
        }

        public BlendState BlendState => BlendState.Additive;
        public HJScarletDrawLayer LayerToRenderTo => HJScarletDrawLayer.BeforeDusts;
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            HJScarletMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D tex = HJScarletTexture.Texture_StandardGradient.Value;
            Effect e = HJScarletShader.AlphaFade;
            e.Parameters["uFadeoutLeftLength"].SetValue(0.3f);
            e.Parameters["uFadeinRigtLength"].SetValue(0.3f);
            e.Parameters["uFadeinTopLength"].SetValue(0.3f);
            e.Parameters["uFadeinBottomLength"].SetValue(0.5f);
            e.Parameters["UVMult"].SetValue(new Vector2(1f, 1f));
            e.CurrentTechnique.Passes[0].Apply();
            TrailFunc(tex, Color.White * 0.60f, 9f);
            TrailFunc(tex, Color.White * 0.20f, 6f);
            TrailFunc(tex, Color.White * .20f, 3f);
            TrailFunc(tex, Color.White * .20f, 2f);

            Vector4 vector4 = new(0.2f, 0.2f, 0.1f, 0.6f);
            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 1.79f * RandValueSummary.X), new Vector2(1f, 0.985f), Color.White);
            Texture2D texture2 = HJScarletTexture.Noise_Aura.Value;
            TrailFunc(texture2, Color.White, 10f);

            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 2.79f * RandValueSummary.Y), new Vector2(2f, 0.975f), Color.White);
            texture2 = HJScarletTexture.Noise_Misc.Value;
            TrailFunc(texture2, Color.White * 0.42f, 10f);
            texture2 = HJScarletTexture.Noise_Aura.Value;

            HJScarletMethods.ApplyAlphaCut(vector4, new(0, -Main.GlobalTimeWrappedHourly * 0.79f * RandValueSummary.Z), new Vector2(3.2f, 1.94f), Color.White);
            TrailFunc(texture2, Color.White * 0.92f, 20f);
            TrailFunc(texture2, Color.White * 0.92f, 10f);
            ApplyTrailAlt(HJScarletTexture.Trail_ManaStreakTiny.Value, Color.DarkGray);
            ApplyTrailAlt(HJScarletTexture.Trail_FadedStreak.Value, Color.Gray, 10);
            ApplyTrailAlt(HJScarletTexture.Trail_TerraRayFlow.Value, Color.WhiteSmoke, 28);

            HJScarletMethods.EndShaderAreaPixel();

        }
        public void ApplyTrailAlt(Texture2D tex, Color color, float primitiveHeight = 30, float heightPosOffset = 0f)
        {
            float laserLength = 150;
            Effect shader = HJScarletShader.TerrarRayLaser;
            shader.Parameters["LaserTextureSize"].SetValue(tex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, tex.Height));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -150);
            shader.Parameters["uColor"].SetValue(color.ToVector4() * 1);
            shader.Parameters["uFadeoutLength"].SetValue(1.13f);
            shader.Parameters["uFadeinLength"].SetValue(0.12f);
            shader.CurrentTechnique.Passes[0].Apply();

            DrawSetting sets = new(tex);
            List<TrailDrawDate> date = [];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 listPos = Projectile.oldPos[i] + Projectile.Size / 2 + Projectile.SafeDir().RotatedBy(PiOver2) * heightPosOffset;
                date.Add(new(listPos, Color.White, new(0, primitiveHeight * 1.92f), Projectile.oldRot[i]));
            }
            TrailRender.DrawTrail(date.ToArray(), sets);
        }

    }
}
