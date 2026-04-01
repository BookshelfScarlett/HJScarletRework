using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Metaballs;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Executor
{
    public class DreamlessNightBeam : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Executor;
        public enum BeamType
        {
            SplitBeam,
            MinionBeam,
            DecorationBeam
        }
        public ref float Timer => ref Projectile.ai[0];
        public AnimationStruct Helper = new AnimationStruct(3);
        public ref float Osci => ref Projectile.ai[2];
        public List<NPC> StoredNPC = [];
        public int TotalNPC = 8;
        public BeamType BeamState
        {
            get => (BeamType)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public NPC CurChasedTarget = null;
        public float RotIncrease = 0;
        public float LastIncrease = 0;
        public float MaxSpeed = 4f;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(16, 2);
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.extraUpdates = 3;
            Projectile.SetupImmnuity(60);
            Projectile.tileCollide = false;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 480;
            Projectile.ignoreWater = true;
        }
        public override bool? CanDamage()
        {
            return Projectile.MeetMaxUpdatesFrame(Timer, 3);
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[AniState.Begin] = 10;
            SpawnDust(Projectile.Center, Projectile.velocity.ToSafeNormalize());
            RandScale = Main.rand.NextFloat(0.75f, 1.36f);
        }
        public override void ProjAI()
        {
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            UpdateParticle();
        }

        private void UpdateParticle()
        {
            switch (BeamState)
            {
                case BeamType.MinionBeam:
                    SpawnMinionBeamParticle();
                    UpdateMininoBeamAI();

                    break;
                case BeamType.SplitBeam:
                    SpawnSplitBeamParticle();
                    break;

                case BeamType.DecorationBeam:
                    SpawnDecorationBeamParticle();
                    break;
            }

        }

        private void SpawnDecorationBeamParticle()
        {
            if (Projectile.FinalUpdate())
                RotIncrease += ToRadians(0.1f);
            RotIncrease = Clamp(RotIncrease, ToRadians(0), ToRadians(1.4f));
            if (RotIncrease - LastIncrease > ToRadians(0.2f))
            {
                LastIncrease = RotIncrease;
                Projectile.netUpdate = true;
            }
            Projectile.velocity = Projectile.velocity.RotatedBy(ToRadians(2f) + RotIncrease);
            if (Projectile.velocity.LengthSquared() > MaxSpeed * MaxSpeed)
                Projectile.velocity *= 0.9f;

            int lifeTime = 80;
            new FusableBall(Projectile.Center.ToRandCirclePos(4f), Projectile.velocity.ToRandVelocity(0.1f, 1.1f, 2.4f), RandLerpColor(Color.DarkViolet, Color.Purple), lifeTime / 4, 0.475f, new Vector2(1.25f, 0.5f) * 0.230f).SpawnToPriority();
            int dCount = PerformanceMode ? 2 : 4;
            for (int i = 0; i < dCount; i++)
            {
                Vector2 nebulaPos = Projectile.Center.ToRandCirclePos(2) + Projectile.SafeDir() * i * 2.5f;
                Vector2 nebulaVel = Projectile.velocity.ToRandVelocity(ToRadians(10), 1.1f, 3.4f);
                float nebulaScale = Main.rand.NextFloat(0.1f, 0.125f) * 0.80f;
                ShadowNebula.SpawnParticle(nebulaPos, nebulaVel, nebulaScale, HJScarletTexture.Texture_WhiteCircle.Value);
            }
            if (PerformanceMode && Main.rand.NextBool())
                return;
            if (Main.rand.NextBool(8))
            {
                //尽量远离即可。
                Vector2 starPos = Projectile.Center.ToRandCirclePosEdge(20);
                Vector2 starVel = Projectile.velocity.ToRandVelocity(ToRadians(5), 0.1f, 0.4f);
                Color starColor = RandLerpColor(Color.DarkViolet, Color.Violet);
                new KiraStar(starPos, starVel, starColor, 60, 0, 1, 0.25f, fadeIn: true).SpawnToNonPreMult();
                new FusableBall(starPos, starVel, RandLerpColor(Color.DarkViolet, Color.Purple), 60, 0.475f, new Vector2(0.5f, 0.5f) * 0.230f).SpawnToPriority();
            }
            if (Main.rand.NextBool(4))
            {
                Vector2 starPos = Projectile.Center.ToRandCirclePosEdge(4);
                Vector2 starVel = Projectile.velocity.ToRandVelocity(ToRadians(30f), 1.1f, 2.4f);
                Color starColor = RandLerpColor(Color.DeepPink, Color.Violet);
                float scale = Main.rand.NextFloat(0.30f, 0.36f);
                new ShinyCrossStar(starPos, starVel, starColor, lifeTime, RandRotTwoPi, 1f, scale, false,0.1f).Spawn();
            }
        }

        public void UpdateMininoBeamAI()
        {
            if (Main.rand.NextBool(8))
            {
                //尽量远离即可。
                Vector2 starPos = Projectile.Center.ToRandCirclePosEdge(20);
                Vector2 starVel = Projectile.velocity.ToRandVelocity(ToRadians(5), 0.1f, 0.4f);
                Color starColor = RandLerpColor(Color.DarkViolet, Color.Violet);
                new KiraStar(starPos, starVel, starColor, 60, 0, 1, 0.25f, fadeIn: true).SpawnToNonPreMult();
                new FusableBall(starPos, starVel, RandLerpColor(Color.DarkViolet, Color.Purple), 60, 0.475f, new Vector2(0.5f, 0.5f) * 0.230f).SpawnToPriority();
            }

        }

        public float RandScale = 0f;
        private void SpawnDust(Vector2 spawnPos, Vector2 dir)
        {
        }
        public void SpawnMinionBeamParticle()
        {
            int lifeTime = 80;
            new FusableBall(Projectile.Center.ToRandCirclePos(4f), Projectile.velocity.ToRandVelocity(0.1f, 1.1f, 2.4f), RandLerpColor(Color.DarkViolet, Color.Purple), lifeTime / 4, 0.675f, new Vector2(1.25f, 0.5f) * 0.230f).SpawnToPriority();
            int dCount = PerformanceMode ? 2 : 4;
            for (int i = 0; i < dCount; i++)
            {
                Vector2 nebulaPos = Projectile.Center.ToRandCirclePos(2) + Projectile.velocity / dCount * i + RandVelTwoPi(1.1f,1.6f);
                Vector2 nebulaVel = RandVelTwoPi(0, 0.2f) + Projectile.SafeDir();
                Vector2 nebulaScale = Main.rand.NextFloat(0.136f, 0.148f) * Vector2.One;
                ShadowNebulaVector2.SpawnParticle(nebulaPos, nebulaVel, nebulaScale, Projectile.velocity.ToRotation(), 100, HJScarletTexture.Texture_WhiteCircle.Value);
            }
            if (PerformanceMode && Main.rand.NextBool())
                return;

            if (Main.rand.NextBool(3))
            {
                Vector2 starPos = Projectile.Center.ToRandCirclePos(2);
                Vector2 starVel = Projectile.velocity.ToRandVelocity(0, 0.5f, 0.9f);
                Color starColor = RandLerpColor(Color.DeepPink, Color.Violet);
                float scale = Main.rand.NextFloat(0.38f, 0.42f);
                new ShinyCrossStar(starPos, starVel, starColor, lifeTime, RandRotTwoPi, 1f, scale, false).Spawn();
            }
            if (Main.rand.NextBool(4))
            {
                Vector2 starPos = Projectile.Center.ToRandCirclePosEdge(4);
                Vector2 starVel = Projectile.velocity.ToRandVelocity(ToRadians(30f), 1.1f, 2.4f);
                Color starColor = RandLerpColor(Color.DeepPink, Color.Violet);
                float scale = Main.rand.NextFloat(0.30f, 0.36f);
                new ShinyCrossStar(starPos, starVel, starColor, lifeTime, RandRotTwoPi, 1f, scale, false,0.1f).Spawn();
            }

        }
        public void SpawnSplitBeamParticle()
        {
            if (Projectile.FinalUpdate())
                RotIncrease += ToRadians(0.05f);
            RotIncrease = Clamp(RotIncrease, ToRadians(0), ToRadians(1.2f));
            if (RotIncrease - LastIncrease > ToRadians(0.2f))
            {
                LastIncrease = RotIncrease;
                Projectile.netUpdate = true;
            }
            //转向
            Projectile.velocity = Projectile.velocity.RotatedBy(ToRadians(2f) + RotIncrease);
            //控速
            if (Projectile.velocity.LengthSquared() > MaxSpeed * MaxSpeed)
                Projectile.velocity *= 0.9f;
            //粒子。
            int lifeTime = 80;
            new FusableBall(Projectile.Center.ToRandCirclePos(4f), Projectile.velocity.ToRandVelocity(0.1f, 1.1f, 2.4f), RandLerpColor(Color.DarkViolet, Color.Purple), lifeTime / 4, 0.475f, new Vector2(1.25f, 0.5f) * 0.230f).SpawnToPriority();
            int count = PerformanceMode ? 2 : 4;
            for (int i = 0; i < count; i++)
            {
                Vector2 nebulaPos = Projectile.Center.ToRandCirclePos(2) + Projectile.SafeDir() * i * 2.5f;
                Vector2 nebulaVel = Projectile.velocity.ToRandVelocity(ToRadians(10), 1.1f, 3.4f);
                float nebulaScale = Main.rand.NextFloat(0.1f, 0.125f) * 0.80f;
                ShadowNebula.SpawnParticle(nebulaPos, nebulaVel, nebulaScale, HJScarletTexture.Texture_WhiteCircle.Value);
            }
            if (PerformanceMode && Main.rand.NextBool())
                return;

            if (Main.rand.NextBool(4))
            {
                Vector2 starPos = Projectile.Center.ToRandCirclePosEdge(4);
                Vector2 starVel = Projectile.velocity.ToRandVelocity(ToRadians(5), 1.1f, 2.4f);
                Color starColor = RandLerpColor(Color.DeepPink, Color.Violet);
                new ShinyCrossStar(starPos, starVel, starColor, lifeTime, RandRotTwoPi, 1f, 0.30f, false).Spawn();
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return base.CanHitNPC(target);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
