using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class DialecticsCubeProj :HJScarletFriendlyProj
    {
        public override ClassCategory Category => ClassCategory.Melee;
        public override string Texture => HJScarletTexture.Specific_DialectCube.Path;   
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(4, 2);
        }
        public enum Style
        {
            Slowdown,
            Spawn,
            HomeToTarget
        }
        public Style AttackType
        {
            get => (Style)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[1];
        public Vector2 MountedPos = Vector2.Zero;
        public override void ExSD()
        {
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void AI()
        {
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5f, 5f), DustID.BlueTorch);
                d.noGravity = true;
            }
            switch(AttackType)
            {
                case Style.Slowdown:
                    DoSlowDown();
                    break;
                case Style.Spawn:
                    DoSpawn();
                    break;
                case Style.HomeToTarget:
                    DoHomeToTarget();
                    break;
            }
        }

        private void DoSlowDown()
        {
            //减速
            Projectile.velocity *= 0.8f;
            if (Projectile.velocity.Length() < 3f)
            {
                //将当期位置缓存，并跳转状态
                MountedPos = Projectile.Center;
                AttackType = Style.Spawn;
                //刷新一次生命
                Projectile.timeLeft = 500;
                Projectile.extraUpdates = 0;
                Projectile.penetrate = 1;
            }
        }

        private void DoSpawn()
        {
            //递增的值越大，摆动幅度越大
            Timer += 0.025f;
            Projectile.timeLeft = 500;
            //基本的挂机状态，注意这里取用的永远为射弹在上方速度取零时的位置
            //这里会记录当前方块的转角，用来做摆动的处理
            Vector2 anchorPos = MountedPos + Projectile.rotation.ToRotationVector2().RotatedBy(PiOver2) * (MathF.Sin(Timer) / 9f) * 180f;
            //实际更新位置
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, 0.15f);
            if (Timer > 0.40f)
            {
                Projectile.extraUpdates = 2;
                AttackType = Style.HomeToTarget;
                Projectile.netUpdate = true;
            }
        }
        public override bool? CanDamage() => AttackType == Style.HomeToTarget;
        public override bool PreKill(int timeLeft)
        {
            return base.PreKill(timeLeft);
        }
        private void DoHomeToTarget()
        {
            if (!Projectile.GetTargetSafe(out NPC target, true, 600))
            {
                Projectile.Kill();
                return;
            }
            Projectile.HomingTarget(target.Center, 1800, 20f, 30f);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.White);
            Projectile.DrawProj(Color.White);
            return false;
        }
    }
}
