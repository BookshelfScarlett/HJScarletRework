using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Ranged
{
    public class TheMossHeldLock : ThrownHammerLockProj
    {
        public override string Texture => GetInstance<TheMossMainProj>().Texture;
        public override int HitBoxSD => 50;
        public override int MountedTime => 10;
        public ref float Timer => ref Projectile.ai[0];
        public float MaxAnimationTime = 15f;
        public override void ExSD()
        {
            Projectile.ownerHitCheck = true;
            Projectile.tileCollide = false;
            //画师和写代码的要打架了
            Projectile.scale *= 1.1f;
            Projectile.penetrate =-1;
            Projectile.extraUpdates = 0;
            base.ExSD();
        }
        public override void AI()
        {
            Projectile.velocity *= 0f;
            Timer++;
            if (Timer > MaxAnimationTime)
                Timer = MaxAnimationTime;
            if (Projectile.timeLeft > GetSeconds(2))
            {
                FollowMouse();
            }
            else
            {
                Projectile.rotation += ToRadians(25f);
                Projectile.HomingTarget(Owner.Center, -1, 16f, 20f);
                if (Projectile.Hitbox.Intersects(Owner.Hitbox))
                    Projectile.Kill();
            }
        }
        /// <summary>
        /// 这段码从魔法飞弹移植过来
        /// 相对安全的跟随鼠标写法
        /// </summary>
        float Oscillation = 0;
        private void FollowMouse()
        {
            Vector2 center = Owner.MountedCenter;
            //递增的值越大，锤子的摆动幅度越大
            Oscillation += 0.025f;
            //基本的挂机状态，此处使用了正弦曲线来让锤子常规上下偏移
            float anchorX = Owner.MountedCenter.X;
            float anchorY = Owner.MountedCenter.Y -(90f + 60f * (MathF.Sin(Oscillation) / 9f));
            Vector2 anchorPos = new(anchorX, anchorY);
            //实际更新位置
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, 0.15f);
            //计算锤子需要的朝向。
            //这里会依据玩家是否按下左键来使朝向取反，即按住左键的时候，锤头朝向指针，其他情况下，锤柄朝向玩家
            float angleToWhat = ((Owner.LocalMouseWorld() - Projectile.Center)).SafeNormalize(Vector2.One).ToRotation();
            //最后使用lerp来让锤子朝向得到修改。
            Projectile.rotation = (angleToWhat);

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SB.Draw(Projectile.GetTexture(), Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + PiOver4 + Pi, Projectile.GetTexture().Size() / 2, Projectile.scale, 0, 0);
            return false;
        }
    }
}
