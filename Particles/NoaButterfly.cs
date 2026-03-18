using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace HJScarletRework.Particles
{
    public class NoahButterfly : BaseParticle
    {
        private Vector2 BeginVector;
        private float Speed;
        private float SeedValue;
        //控制摆动/浮动
        private float HorizonalFloting;
        private float VerticalFloating;
        //振翅速度
        private float WingFlapSpeed;
        //用于关联帧动画
        private float swingOffsetX;
        private bool NoLighting;
        private bool DrawGlowingOrbParticle;
        public NoahButterfly(Vector2 position, Vector2 beginVector, Color color, int lifeTime, float opacity, float scale, float speed, bool noLighting = false, bool drawGlowingOrbParticle= false)
        {
            Position = position;
            BeginVector = beginVector;
            DrawColor = color;
            Opacity = opacity;
            Lifetime = lifeTime;
            Scale = scale;
            Speed = speed;
            HorizonalFloting = Main.rand.NextFloat(0.5f, 1.2f);
            VerticalFloating = Main.rand.NextFloat(0.3f, 0.8f);
            WingFlapSpeed = Main.rand.NextFloat(1.8f, 2.5f);
            NoLighting = noLighting;
            DrawGlowingOrbParticle = drawGlowingOrbParticle;
        }
        public override void OnSpawn()
        {
            SeedValue = Main.rand.Next(0, 100000);
        }
        public override void Update()
        {
            if (Speed != 0)
            {

                //基础移动
                float currentSpeed = Speed * (1 - LifetimeRatio); // 后期减速70%
                Vector2 baseMovement = BeginVector.SafeNormalize(Vector2.One) * currentSpeed;

                //横向浮动
                swingOffsetX = (float)Math.Sin(Time * 0.05f + SeedValue * 0.0001f) * HorizonalFloting;

                //纵向浮动
                float floatOffsetY = (float)Math.Cos(Time * 0.07f + SeedValue * 0.0002f) * VerticalFloating;

                //叠加速度
                Velocity = baseMovement + new Vector2(swingOffsetX, floatOffsetY);

                //轻微旋转：随摆动调整角度，增强真实感
                Rotation = BeginVector.ToRotation() + swingOffsetX * 0.02f;
                //每100帧有概率变向
                if (Main.rand.NextBool(100)) 
                    BeginVector = BeginVector.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f));

                //透明度随生命周期渐变
                Opacity = LifetimeRatio < 0.1f ? Lerp(0f, 1, Opacity) : LifetimeRatio > 0.9f ? (1 - LifetimeRatio) * 10 : 1f;
            }
            //是的，这里实际上也会尝试在粒子里面生成粒子。
            if (DrawGlowingOrbParticle)
            {
                if (Main.rand.NextBool(4))
                {
                    for (int i = 0; i < 2; i++)
                        new ShinyOrbParticle(Position.ToRandCirclePos(6f), -Velocity * 0.6f, DrawColor, 30, Scale).Spawn();
                }
            }
            Scale *= 0.98f;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = HJScarletTexture.Particle_NoahButterfly.Value;

            float flapProgress = (LifetimeRatio * WingFlapSpeed + Math.Abs(swingOffsetX) * 0.5f + SeedValue * 0.00001f) % 1f;
            int frameindex = (int)(flapProgress * 4);
            Rectangle frame = HJScarletTexture.Particle_NoahButterfly.Texture.Frame(2, 2, frameindex % 2, frameindex / 2);
            Vector2 origin = frame.Size() * 0.5f;

            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, DrawColor * Opacity, Rotation + ToRadians(95f), origin, Scale, 0, 0f);
        }
    }
}
