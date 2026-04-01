using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class AzureFrostmarkProj : ThrownSpearProjClass
    {
        public override string Texture => ProjPath + "Proj_" + nameof(AzureFrostmark);
        public enum Style
        {
            Shoot,
            SpinAndFade
        }
        public Style AttackType
        {
            get => (Style)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[1];
        public float Speed = 0f;
        public float SpinMoveTime = 60f;
        public bool HitTarget = false;
        public int SpawnTime = 0;
        public override void ExSSD()
        {
            Projectile.ToTrailSetting(8, 2);
        }

        public override void ExSD()
        {
            Projectile.height = Projectile.width = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 1;
            Projectile.Opacity = 0f;
            Projectile.scale = 0f;
            Projectile.timeLeft = 300;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Ice);
            if (!Projectile.HJScarlet().FirstFrame)
            {
                Speed = Projectile.velocity.Length();
                Projectile.originalDamage = Projectile.damage;
            }
            UpdateParticles();
            switch (AttackType)
            {
                case Style.Shoot:
                    DoShoot();
                    break;
                case Style.SpinAndFade:
                    DoSpinAndFade();
                    break;
            }
        }
        public void UpdateParticles()
        {
            Projectile.Opacity = Lerp(Projectile.Opacity, 1f, 0.12f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.scale = Lerp(Projectile.scale, 1f, 0.2f);
            Vector2 dustVel = Projectile.velocity.ToRandVelocity(ToRadians(25f), 1.2f, 1.6f);
            //timer会同时总控射弹的速度，这里需要随射弹尽量延展粒子生成来避免过度集中
            float ratios = Lerp(1f, 0f, Timer / 60f);
            Vector2 spawnPos = Projectile.Center.ToRandCirclePosEdge(2f);
            Color smokeColor = RandLerpColor(Color.DeepSkyBlue, Color.WhiteSmoke) * Clamp(ratios, 0.8f, 1f);
            if (Main.rand.NextBool())
                new SmokeParticle(spawnPos, dustVel, smokeColor, 40, RandRotTwoPi, 1f, Main.rand.NextFloat(0.14f, 0.18f)).Spawn();
            //钝角。
            spawnPos = Projectile.Center.ToRandCirclePosEdge(6f);
            if (Main.rand.NextBool())
            {
                new ShinyOrbParticle(spawnPos, dustVel, RandLerpColor(Color.RoyalBlue, Color.DeepSkyBlue) * ratios, 40, 0.6f).Spawn();
                new ShinyOrbParticle(spawnPos, dustVel, RandLerpColor(Color.White, Color.WhiteSmoke) * ratios, 40, 0.2f).Spawn();
            }
        }
        private void DoShoot()
        {
            Timer++;
            if (Timer % 20 == 0)
            {
                if (SpawnTime > 1)
                {
                    AttackType = Style.SpinAndFade;
                    Projectile.netUpdate = true;
                    return;
                }
                SoundEngine.PlaySound(SoundID.Item109 with {Volume = 0.8f, MaxInstances = 1, Pitch = 0.30f + SpawnTime * 0.3f }, Projectile.Center);
                SpawnTime++;
                SpawnEnergyBall();
            }
        }
        //这段真的是沉浸于自己的世界了
        private void DoSpinAndFade()
        {
            SpawnEnergyBall();
            SpawnPreKillParticle();
            SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact with { MaxInstances = 1, Pitch = 0.30f}, Projectile.Center);
                Projectile.Kill();
        }
        private void SpawnEnergyBall()
        {
            if (!Projectile.IsMe())
                return;
            //这里需要遍历一遍所取位置是否处于wall里面。如果是则回退直到适合为止
            Vector2 projDir = Projectile.SafeDirByRot();
            Vector2 spawnPos = Projectile.Center;
            //这里主要是为了对准矛上的柄。不过如果射弹本身就处于物块检测内，那就没法子了
            Vector2 posOffset = projDir * 20f;
            Vector2 dir = Vector2.UnitY + Projectile.velocity.ToSafeNormalize() * 0.5f;
            Projectile ball = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), posOffset + spawnPos, dir * -Main.rand.NextFloat(4f, 7.2f), ProjectileType<AzureFrostmarkEnergy>(), Projectile.damage / 2, Projectile.knockBack);
            //天王老子来了我都要用自己的粒子
            //不服憋着
            for (int j = 0; j < 15; j++)
            {
                //最大的原因是白天过曝看不到
                //这里的光球粒子需要携带一个暗色的烟出来
                Vector2 pos = posOffset + spawnPos.ToRandCirclePos(8f);
                Vector2 vel = dir * Main.rand.NextFloat(4f);
                //由于粒子绘制原因。shinyobr的速度需要更短
                new ShinyOrbParticle(pos, vel * 0.8f, RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, 0.7f).Spawn();
                new SmokeParticle(pos, vel, RandLerpColor(Color.DeepSkyBlue, Color.Gray), 40, 0.5f, 1f, 0.14f).SpawnToPriorityNonPreMult();
            }
            new CrossGlow(spawnPos + posOffset, Color.RoyalBlue, 40, 1f, 0.22f).Spawn();
            new CrossGlow(spawnPos + posOffset, Color.White, 40, 1f, 0.15f).Spawn();
            for (int i = 0; i < 15; i++)
                new SmokeParticle(posOffset + spawnPos.ToRandCirclePosEdge(4f), RandDirTwoPi * 1f, RandLerpColor(Color.DeepSkyBlue, Color.Gray), 40, RandRotTwoPi, 1f, 0.24f).SpawnToPriorityNonPreMult();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SpawnPreKillParticle();
            Projectile.Kill();
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 12;i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(6f);
                Vector2 vel = Projectile.velocity.ToRandVelocity(ToRadians(20f), 1.2f, 8.9f);
                new KiraStar(pos, vel, RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, 0, 1, 0.18f).Spawn();
                new KiraStar(pos, vel, Color.White, 40, 0, 1, 0.10f).Spawn();
            }
            for (int i = 0; i < 12;i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(6f);
                Vector2 vel = Projectile.velocity.ToRandVelocity(ToRadians(20f), 1.2f, 8.9f);
                new ShinyCrossStar(pos, vel, RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, RandRotTwoPi, 1, Main.rand.NextFloat(0.34f, 0.48f), false).Spawn();

            }
            HitTarget = true;
        }
        public void SpawnPreKillParticle()
        {
            for (int i = 0;i<16;i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(4f) + Projectile.SafeDir() * Main.rand.NextFloat(-22f, 12f) + Projectile.SafeDir() * 20f;
                Vector2 vel = Projectile.SafeDir() * Main.rand.NextFloat(-2f, 2f);
                new TurbulenceShinyOrb(pos, Main.rand.NextFloat(-2f, 1.5f), RandLerpColor(Color.RoyalBlue, Color.DeepSkyBlue), 40, Main.rand.NextFloat(0.65f, 0.85f), RandRotTwoPi).Spawn();
            }
            for (int i = 0;i<16;i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(4f) + Projectile.SafeDir() * Main.rand.NextFloat(-22f, 12f) + Projectile.SafeDir() * 20f;
                Vector2 vel = Projectile.SafeDir() * Main.rand.NextFloat(-2f, 2f) + RandVelTwoPi(1f,3f);
                new KiraStar(pos, vel, RandLerpColor(Color.RoyalBlue, Color.SkyBlue), 40, 0, 1, 0.18f).Spawn();
                new KiraStar(pos, vel, Color.White, 40, 0, 1, 0.10f).Spawn();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            int length = Projectile.oldPos.Length;
            float rot = Projectile.rotation;
            Vector2 offset = Projectile.SafeDir() * 60f;
            drawPos -= offset;
            float ratios = Lerp(1f, 0f, Timer / 60f);
            for (int i = length - 1; i >= 0; i--)
            {
                if (AttackType == Style.SpinAndFade)
                    continue;
                float rads = (float)i / length;
                Color lerpColor = Color.Lerp(Color.DeepSkyBlue, Color.Lerp(Color.SkyBlue, Color.AliceBlue, rads * 0.7f), rads).ToAddColor(0) * Clamp(Projectile.velocity.Length(), 0f, 1f) * 0.50f;
                SB.Draw(projTex, Projectile.oldPos[i] + Projectile.PosToCenter() - offset, null, lerpColor * ratios, rot + PiOver4, ori, Projectile.scale, 0, 0);
            }
            for (int i = 0; i < 8; i++)
                SB.Draw(projTex, drawPos + ToRadians(60f * i).ToRotationVector2() * 1.5f, null, Color.SkyBlue.ToAddColor(0) * ratios, rot + PiOver4, ori, Projectile.scale, 0, 0);
            SB.Draw(projTex, drawPos, null, Color.WhiteSmoke * ratios, rot + PiOver4, ori, Projectile.scale, 0, 0);
            return false;
        }
    }
}
