using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class FierySpearFireball : HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(20, 2);
        }
        public ref float Timer => ref Projectile.ai[0];
        public int BounceTime
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = 1;
            Projectile.noEnchantmentVisuals = true;
            Projectile.timeLeft = GetSeconds(3);
        }
        private float SearchDistance = 260f;
        private int TotalBounceTime = 2;
        private float KillDistance = 1800f;
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Orange);
            //如果开了灾厄，则加强索敌距离，生存时间和提供1eu
            Projectile.rotation = Projectile.velocity.ToRotation();

            Timer++;
            bool additionRequire = (Timer > 15f && Projectile.HJScarlet().GlobalTargetIndex != -1) || BounceTime > 0;
            float homingSpeed = Projectile.HJScarlet().GlobalTargetIndex != -1 ? 12f : 6f;
            if (Projectile.GetTargetSafe(out NPC target, true, SearchDistance) && additionRequire)
                Projectile.HomingTarget(target.Center, -1, homingSpeed, 20f);
            else
            {
                Projectile.AffactedByGrav(velMult: 0.98f, yAdd: 0.32f, maxGravSpeed: 12f);
            }
            //粒子
            if (Main.rand.NextBool(3))
            {
                Vector2 spawnPos = Projectile.Center.ToRandCirclePosEdge(4);
                Vector2 speed = Projectile.SafeDirByRot() * Main.rand.NextFloat(1.2f, 1.9f);
                Dust d = Dust.NewDustPerfect(spawnPos, DustID.Torch);
                d.velocity = speed;
                d.position += Projectile.SafeDirByRot(90) * 1.2f;
                d.scale *= 1.6f;
                d.noGravity = true;
            }
            if (Main.rand.NextBool())
            {
                new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(4), Vector2.UnitY * -Main.rand.NextFloat(0.5f, 1.3f), RandLerpColor(Color.Orange, Color.OrangeRed), 40, 0, 1, 0.45f, false).Spawn();
            }

            //距离玩家过远时处死
            if (Projectile.TooAwayFromOwner(KillDistance))
                Projectile.Kill();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, GetSeconds(5));
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Math.Abs(oldVelocity.Y) > 5f)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                for (int i = 0; i < 8; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.FlameBurst);
                    dust.noGravity = true;
                    dust.scale *= 2f * Projectile.scale;
                }
            }
            Projectile.BounceOnTile(oldVelocity);
            BounceTime += 1;
            return BounceTime > TotalBounceTime;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                new TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(16f), 1.2f, RandLerpColor(Color.Orange, Color.OrangeRed), 20, 0.5f, RandRotTwoPi).Spawn();
            }
            base.OnKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 projPos, out Vector2 ori);
            projTex = HJScarletTexture.Particle_ShinyOrbHard.Value;
            ori = HJScarletTexture.Particle_ShinyOrbHard.Origin;
            Color baseColor = Color.DarkOrange;
            Color targetColor = Color.OrangeRed;
            //绘制残影
            float oriScale = 0.70f;
            float scale = 1f;
            int length = 10;
            for (int i = 0; i < length; i++)
            {
                scale *= 0.925f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(baseColor, targetColor, (1 - rads)).ToAddColor(50) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads);
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.20f);
                float rot = Lerp(Projectile.oldRot[i], Projectile.oldRot[0], 1f);
                SB.Draw(projTex, lerpPos + Projectile.PosToCenter(), null, edgeColor, rot, ori, oriScale * scale * Projectile.scale, 0, 0);
            }

            SB.Draw(projTex, projPos, null, Color.Orange.ToAddColor(50), Projectile.rotation, ori, oriScale * Projectile.scale, 0, 0);
            SB.Draw(projTex, projPos, null, Color.White.ToAddColor(0), Projectile.rotation, ori, oriScale * Projectile.scale * 0.85f, 0, 0);
            return false;
        }
        public override bool? CanDamage()
        {
            return Timer > 20f;
        }
    }
}
