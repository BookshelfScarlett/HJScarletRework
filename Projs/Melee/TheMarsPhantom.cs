using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.ParticleSystem;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Melee
{
    public class TheMarsPhantom : HJScarletFriendlyProj
    {
        public override string Texture => ProjPath + GetType().Name;
        public override ClassCategory Category => ClassCategory.Melee;
        public ref float Timer => ref Projectile.ai[0];
        public enum Style
        {
            Shoot,
            Fade
        }
        public Style AttackType
        {
            get => (Style)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public int TotalShootTime = 0;
        public override void ExSD()
        {
            Projectile.extraUpdates = 3;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.Opacity = 0f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.noEnchantmentVisuals = true; 
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
            if (!Projectile.HJScarlet().FirstFrame)
                InitParticle();
            Projectile.Opacity = Clamp(Projectile.Opacity, 0, 1);
            Projectile.rotation = Projectile.velocity.ToRotation();

            GeneralParticle();

            if (AttackType == Style.Shoot)
            {
                Projectile.Opacity += 0.2f;
                //注意这里：这里会一直锁住追踪目标
                if (Projectile.GetTargetSafe(out NPC target, false))
                    Projectile.HomingTarget(target.Center, -1, 11f, 20f, 10);
                //否则，直接处死他，模拟波涌剑气行为
                else
                {
                    AttackType = Style.Fade;
                    //直接将当前剑气标记为超过3次命中次数，避免后续生成
                    TotalShootTime = 9;
                }
            }
            else
            {
                Projectile.Opacity -= 0.05f;
                if (Projectile.Opacity <= 0f)
                    Projectile.Kill();
            }
        }
        public void InitParticle()
        {
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX) * Projectile.scale;
            for (float i = 0; i < 18f; i++)
            {
                Vector2 dir2 = ToRadians(360f / 18f * i).ToRotationVector2() * Projectile.scale;
                dir2.X /= 3.6f;
                dir2 = dir2.RotatedBy(Projectile.velocity.ToRotation());
                Vector2 pos = Projectile.Center + dir * 12f + dir2 * 18f;
                ShinyOrbParticle shinyOrbParticle = new ShinyOrbParticle(pos, dir2 * 1f, RandLerpColor(Color.DeepSkyBlue, Color.White), 40, 0.3f * (3.5f - Math.Abs(8f - i) / 2f), BlendStateID.Additive);
                shinyOrbParticle.Spawn();
            }
        }
        public void GeneralParticle()
        {
            Color fireColor = RandLerpColor(Color.DeepSkyBlue, Color.White);
            Vector2 firePos = Projectile.Center + Main.rand.NextVector2CircularEdge(3f, 3f);
            new StarShape(firePos, -Projectile.SafeDirByRot() * 1.2f, fireColor * Projectile.Opacity, 0.6f, 30).Spawn();
            int i = 0;
            while (i < 2)
            {
                Color orbColor = RandLerpColor(Color.DeepSkyBlue, Color.White) * Projectile.Opacity;
                Vector2 orbPos = Projectile.Center + Main.rand.NextVector2CircularEdge(2f, 2f);
                new TurbulenceShinyCube(orbPos, -Projectile.velocity / 8f, orbColor, 20, 0f, Projectile.Opacity, 0.24f, randPosMoveValue: 4).Spawn();
                i++;
            }

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //等一下处理
            //如果已经生成了超过3个幻影投矛，做掉下方所有的ai
            SoundEngine.PlaySound(HJScarletSounds.TheMars_Hit with { MaxInstances = 1, PitchVariance = 0.2f }, Projectile.Center);
            AttackType = Style.Fade;
            Timer = 0;
            if (TotalShootTime > 1)
                return;
            //随机取当前射弹结束的位置+
            Vector2 projPos = target.Center + Vector2.UnitY.RotatedByRandom(TwoPi) * Main.rand.Next(150, 200);
            Vector2 vel = (target.Center - projPos).SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(3f, 6f);
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), projPos, vel, Type, Projectile.damage, Projectile.knockBack, Owner.whoAmI);
            ((TheMarsPhantom)proj.ModProjectile).TotalShootTime = TotalShootTime + 1;
            proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawProj(Color.Azure * Projectile.Opacity, rotFix:PiOver4);
            return false;
        }
    }
}
