using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Primitives.Trail;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class DeathTollsArrow : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => GetInstance<ShadowSpear>().Texture;
        private enum DoType
        {
            IsSpawned,
            IsHit
        }
        public NPC CurTarget = null;
        //新建一个
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (int)value;
        }
        private ref float AttackTimer => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 36;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 360;
            Projectile.friendly = true;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }
        public override void AI()
        {
            //直接在AI里写死刷新，下方会手动控制这个射弹的处死逻辑
            Lighting.AddLight(Projectile.Center, TorchID.White);
            Projectile.rotation = Projectile.velocity.ToRotation();
            UpdateParticles();
            UpdateAttackAI();

        }

        public void UpdateAttackAI()
        {
            switch (AttackType)
            {
                case DoType.IsSpawned:
                    DoSpawned();
                    break;
                case DoType.IsHit:
                    DoHit();
                    break;
            }
        }
        public void DoSpawned()
        {
            if (CurTarget.IsLegal())
                Projectile.HomingTarget(CurTarget.Center, -1, 20, 20);
            else
            {
                if (Projectile.velocity.LengthSquared() < 20f * 20f)
                    Projectile.velocity *= 1.1f;
                else
                    Projectile.velocity *= 0.95f;
            }
        }

        public void UpdateParticles()
        {
            if (Projectile.IsOutScreen())
                return;
            float ratios = Projectile.scale;
            if (Main.rand.NextBool(6))
                new EmptyRing(Projectile.Center.ToRandCirclePos(4f), Projectile.velocity.ToRandVelocity(ToRadians(5f), 1.2f, 6f) * ratios + Projectile.velocity / 8f, RandLerpColor(Color.DarkViolet, Color.Violet), 40, Main.rand.NextFloat(0.45f, 0.68f) * 0.25f * ratios, 1f, altRing: Main.rand.NextBool()).SpawnToNonPreMult();
            if (Main.rand.NextBool(6))
                new KiraStar(Projectile.Center.ToRandCirclePos(5f), Projectile.velocity.ToRandVelocity(ToRadians(5f), 1.2f, 4f) * ratios + Projectile.velocity / 8f, RandLerpColor(Color.DarkViolet, Color.Purple), 40, 0, 1, 0.25f * ratios).SpawnToNonPreMult();
            if (Main.rand.NextBool(4))
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(4f), Projectile.velocity / 8f * ratios, RandLerpColor(Color.DarkViolet, Color.Purple), 40, 0.65f * ratios).Spawn();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackType != DoType.IsSpawned)
                return;
            for(int i = 0;i<8;i++)
            {
                new SmokeParticle(Projectile.Center.ToRandCirclePos(2), RandVelTwoPi(1f, 6f), RandLerpColor(Color.DarkViolet, Color.Black), 40, RandRotTwoPi, 1f, 0.16f, Main.rand.NextBool()).SpawnToNonPreMult();
            }
            for(int i = 0;i<8;i++)
            {
                new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(4f), RandVelTwoPi(1f,6.5f), RandLerpColor(Color.DarkViolet, Color.Purple), 40, 0.65f).Spawn();
            }
            SoundEngine.PlaySound(HJScarletSounds.Misc_SwordHit with { Pitch = -0.5f, MaxInstances = 1 }, Projectile.Center);
            Projectile.netUpdate = true;
            AttackTimer = 0f;
            AttackType = DoType.IsHit;
        }
        #region AI方法合集
        private void DoHit()
        {
            Projectile.scale = Lerp(Projectile.scale, 0, 0.21f);
            Projectile.Opacity = Lerp(Projectile.Opacity, 0, 0.20f);
            if (Projectile.Opacity <= 0.02f)
                Projectile.Kill();
        }
        #endregion

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 offset = Projectile.SafeDir() * 70f;
            int length = Projectile.oldPos.Length;
            Texture2D star = HJScarletTexture.Particle_SharpTear;
            for (int i = 0; i < length; i++)
            {
                float rads = (float)i / length;
                Vector2 lerpPos = Projectile.oldPos[i] + Projectile.PosToCenter();
                Color drawColor = (Color.Lerp(Color.Purple, Color.DarkOrchid, rads) with { A = 250 }) * 0.9f * (1 - rads) * Projectile.scale * Projectile.Opacity;
                SB.Draw(star, lerpPos, null, drawColor * Clamp(Projectile.velocity.Length(), 0, 1), Projectile.oldRot[i] - PiOver2, star.Size() / 2, Projectile.scale * new Vector2(0.58f, 1.5f), 0, 0);
                SB.Draw(star, lerpPos - Projectile.SafeDir() * 5f, null, drawColor * Clamp(Projectile.velocity.Length(), 0, 1), Projectile.oldRot[i] - PiOver2, star.Size() / 2, Projectile.scale * new Vector2(0.58f, 1.5f), 0, 0);
            }
            SB.EnterShaderArea();
            DrawTrails(HJScarletTexture.Trail_ManaStreakTiny.Texture, Color.DarkViolet, 1.651f, offsetHeight: 0f);
            DrawTrails(HJScarletTexture.Trail_TerraRayFlow.Texture, Color.Violet, 1.605f, offsetHeight: 0f);
            SB.EndShaderArea();

            Projectile.DrawGlowEdge(Color.DarkViolet, rotFix: PiOver4 + PiOver2, drawPosOffset: offset);
            Projectile.DrawProj(Color.Black, offset: .58f, rotFix: PiOver4 + PiOver2, drawPosOffset: offset);

            return false;
        }
        public void DrawTrailsNoShader(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> _, Projectile.oldPos, Projectile.oldRot);
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true, false);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = validPosition.Count;
            for (int j = 0; j < posCount - 1; j++)
            {
                float rot = (validPosition[j + 1] - validPosition[j]).ToRotation();
                Vector2 posOffset = rot.ToRotationVector2().RotatedBy(PiOver2) * offsetHeight;
                trailDrawDates.Add(new(validPosition[j] + Projectile.Size / 2 + posOffset, drawColor, new Vector2(0, 14 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }

        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            Effect shader = HJScarletShader.StandardFlowShader;
            float laserLength = 50;
            shader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -10.2f);
            shader.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue);
            shader.Parameters["uFadeoutLength"].SetValue(0.8f);
            shader.Parameters["uFadeinLength"].SetValue(0.1f);
            shader.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            Projectile.ClearInvaidData(out List<Vector2> validPosition, out List<float> _, Projectile.oldPos, Projectile.oldRot);
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true, false);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = validPosition.Count;
            for (int j = 0; j < posCount - 1; j++)
            {
                float rot = (validPosition[j + 1] - validPosition[j]).ToRotation();
                Vector2 posOffset = rot.ToRotationVector2().RotatedBy(PiOver2) * offsetHeight;
                trailDrawDates.Add(new(validPosition[j] + Projectile.Size / 2 + posOffset, Color.White, new Vector2(0, 8 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }
    }
}