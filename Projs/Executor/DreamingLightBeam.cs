using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.PixelatedRender;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Metaballs;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class DreamingLightBeam : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Executor;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(6, 2);
        }
        public enum BeamType
        {
            Minion,
            Decoration,
            Split
        }
        public AnimationStruct Helper = new(3);
        public BeamType BeamState = BeamType.Decoration;
        public float RotIncrease = 0;
        public float LastIncrease = 0;
        public float MaxSpeed = 5.5f;
        public float BeamLength = 2200;
        public override void ExSD()
        {
            if (BeamState == BeamType.Minion)
            {
                Projectile.width = Projectile.height = 16;
                Projectile.extraUpdates = 8;
                Projectile.timeLeft = 50;
                Projectile.SetupImmnuity(-1);
                Projectile.penetrate = -1;
                Projectile.scale = 0;
                Projectile.Opacity = 0;
                Projectile.tileCollide = false;
                Projectile.ownerHitCheck = true;

            }
            else
            {
                Projectile.width = Projectile.height = 16;
                Projectile.extraUpdates = 1;
                Projectile.SetupImmnuity(60);
                Projectile.tileCollide = false;
                Projectile.penetrate = 4;
                Projectile.timeLeft = 50;
                Projectile.ignoreWater = true;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (BeamState != BeamType.Minion)
            return base.Colliding(projHitbox, targetHitbox);
            else
            {
                if (Projectile.Opacity < 0.1f)
                    return false;
                float _ = float.NaN;
                Vector2 beamEndPos = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * BeamLength;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, beamEndPos, 24f, ref _);
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return BeamState != BeamType.Minion;
        }
        public override void OnFirstFrame()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Helper.MaxProgress[0] = 10;
            Helper.MaxProgress[1] = 5;
            if (BeamState != BeamType.Minion)
                return;
            for (int i = 0; i < 250; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(3f) + Projectile.SafeDirByRot() * 45f  + Projectile.SafeDirByRot() * 6f * i;
                if (HJScarletMethods.OutOffScreen(pos))
                    break;
                if (PerformanceMode && i % 4 == 0)
                    continue;
                    ShadowNebulaAlt.SpawnSharpTearClean(pos, Projectile.SafeDirByRot().ToRandVelocity(0, 1.2f, 1.6f), 0.921f, 60);
                if (Main.rand.NextBool(8))
                {
                    Vector2 vel = RandVelTwoPi(0.4f, 2.6f);
                    new KiraStar(pos, vel, RandLerpColor(Color.DarkViolet, Color.Violet), 40, 0, 1, 0.21f * Main.rand.NextFloat(0.8f, 1.1f)).Spawn();
                    new KiraStar(pos, vel, Color.White, 40, 0, 1, 0.16f * Main.rand.NextFloat(0.8f, 1.1f)).Spawn();

                }
            }
            int offset = PerformanceMode ? 10 : 0;
            for (int i = 0; i < 25 - offset; i++)
            {
                ShadowNebulaAlt.SpawnSharpTearClean(Projectile.Center + Projectile.SafeDirByRot() * 15f, Projectile.velocity.ToRandVelocity(ToRadians(15f), 1f, 28f), 0.56f, 90);
            }
            new CrossGlow(Projectile.Center, Color.DarkViolet, 40, 1, 0.38f).Spawn();
            new CrossGlow(Projectile.Center, Color.Violet, 40, 1, 0.28f).Spawn();
            new CrossGlow(Projectile.Center, Color.Violet, 40, 1, 0.18f).Spawn();
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(2f);
                Vector2 vel = RandVelTwoPi(0.4f, 3.6f);
                new KiraStar(pos, vel, RandLerpColor(Color.DarkViolet, Color.Purple), 60, 0, 1, 0.21f).SpawnToNonPreMult();
                new KiraStar(pos, vel, RandLerpColor(Color.White, Color.Pink), 60, 0, 1, 0.18f).SpawnToNonPreMult();
            }
            offset = PerformanceMode ? 80 : 0;
            for (int i = 0; i < 160- offset; i++)
            {
                float offsetAngle = TwoPi * i / (160f - offset);
                float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 5D) * 1.125f;
                float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 5D) * 0.95f;
                Vector2 puffDustVelocity = new Vector2(unitOffsetX, unitOffsetY) * 6f * 1.0f;
                ShadowNebulaAlt.SpawnCircle(Projectile.Center, puffDustVelocity, 0.13f);
            }
            for (int i = 0; i < 160 - offset; i++)
            {
                float offsetAngle = TwoPi * i / (160f - offset);
                float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 7D) * 1.125f;
                float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 7D) * 1.125f;
                Vector2 puffDustVelocity = new Vector2(unitOffsetX, unitOffsetY).RotatedBy(PiOver4) * 4f * 1.0f;
                ShadowNebulaAlt.SpawnCircle(Projectile.Center, puffDustVelocity, 0.13f);
            }
            for (int i = 0; i < 4; i++)
            {
                float offsetAngle = TwoPi * i / 4f + PiOver2;
                Vector2 vector = offsetAngle.ToRotationVector2() * 3f * 1.0f;
                for (int j = -8; j < 24; j++)
                    ShadowNebulaAlt.SpawnCircle(Projectile.Center, vector + vector * (j / 18f), 0.13f);
            }
        }
        public override void ProjAI()
        {
            switch (BeamState)
            {
                case BeamType.Decoration:
                    SpawnDecorationBeamParticle();
                    break;
                case BeamType.Minion:
                    SpawnMinionBeamParticle();
                    break;
                case BeamType.Split:
                    SpawnSplitBeamParticle();
                    break;
            }

        }

        public void SpawnSplitBeamParticle()
        {
        }

        public void SpawnMinionBeamParticle()
        {
            if (!Helper.IsDone[0])
            {
                Helper.UpdateAniState(0);
                Projectile.Opacity = Lerp(Projectile.Opacity, 1, 0.4f);
                Projectile.scale = Lerp(Projectile.Opacity, 1, 0.2f);
            }
            else if (!Helper.IsDone[1])
            {
                Helper.UpdateAniState(1);
            }
            else
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 0f, 0.03f);
                Projectile.scale = Lerp(Projectile.scale, 0f, 0.05f);
            }

        }

        private void SpawnDecorationBeamParticle()
        {
            if (Projectile.FinalUpdate())
                RotIncrease += ToRadians(0.1f);
            RotIncrease = Clamp(RotIncrease, ToRadians(0), ToRadians(5.8f));
            if (RotIncrease - LastIncrease > ToRadians(0.2f))
            {
                LastIncrease = RotIncrease;
                Projectile.netUpdate = true;
            }
            if (RotIncrease == ToRadians(0.58f))
            {

            }
            else
                Projectile.velocity = Projectile.velocity.RotatedBy(ToRadians(2f) + RotIncrease);
            if (Projectile.velocity.LengthSquared() > MaxSpeed * MaxSpeed)
                Projectile.velocity *= 0.9f;
            else
                Projectile.velocity *= 1.1f;

            int offset = PerformanceMode ? 1 : 0;
            for (int i = 0; i < 3 - offset; i++)
            {
                Vector2 nebulaPos = Projectile.Center.ToRandCirclePos(0f) + Projectile.SafeDir() * i * 1.5f;
                Vector2 nebulaVel = Projectile.SafeDir() * 1.5f;
                float nebulaScale = Main.rand.NextFloat(0.1f, 0.125f) * 1.5f;
                ShadowNebulaAlt.SpawnCircle(nebulaPos, nebulaVel, nebulaScale, 60);
            }
            if (Main.rand.NextBool(6))
            {
                Vector2 starPos = Projectile.Center.ToRandCirclePosEdge(4);
                Vector2 starVel = Projectile.velocity.ToRandVelocity(ToRadians(30f), 0.8f, 1.4f);
                Color starColor = RandLerpColor(Color.DarkViolet, Color.Violet);
                float scale = Main.rand.NextFloat(0.30f, 0.36f);
                new ShinyCrossStar(starPos, starVel, starColor, 80, RandRotTwoPi, 1f, scale, false, 0.1f).Spawn();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overWiresUI.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (BeamState != BeamType.Minion )
                return false;
            if (!Projectile.HJScarlet().FirstFrame)
                return false;
            SB.EnterShaderArea();
            DrawBeam(SB, Color.Lerp(Color.DarkViolet, Color.Purple, 0.5f), 0.12f * Projectile.scale);
            DrawBeam(SB, Color.Lerp(Color.DarkViolet, Color.Violet, 0.55f), 0.10f * Projectile.scale);
            DrawBeam(SB, Color.Lerp(Color.Violet, Color.DarkViolet, 0.62f), 0.08f * Projectile.scale);
            DrawBeam(SB, Color.White, 0.05f * Projectile.scale);
            SB.EndShaderArea();

            PixelatedRenderManager.BeginDrawProj = true;
            return false;
        }
        public void DrawBeam(SpriteBatch sb, Color color, float height)
        {
            Asset<Texture2D> value = HJScarletTexture.Trail_ManaStreak.Texture;
            Effect shader = HJScarletShader.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(value.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(BeamLength, value.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -70);
            shader.Parameters["uColor"].SetValue(color.ToVector4() * Projectile.Opacity);
            shader.Parameters["uFadeoutLength"].SetValue(0.02f);
            shader.Parameters["uFadeinLength"].SetValue(0.02f);
            shader.CurrentTechnique.Passes[0].Apply();
            Vector2 orig = new(0, value.Height() / 2);
            float xScale = BeamLength / value.Width();
            sb.Draw(value.Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, orig, new Vector2(xScale * Clamp(Projectile.scale,0.02f,1f), height * 0.9f), 0, 0);
        }

    }
}
