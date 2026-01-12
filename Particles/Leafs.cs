using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Assets.Registers;

namespace HJScarletRework.Particles
{
    public class Leafs : BaseParticle
    {
        public float Speed = 5f;

        public int SeedOffset = 0;

        public float BeginScale = 1f;

        public int Rotdir = 1;

        // 追踪粒子是否已经着陆
        private bool hasLanded = false;
        public Leafs(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale, float speed)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
            BeginScale = scale;
            Speed = speed;
        }
        public override void OnSpawn()
        {
            SeedOffset = Main.rand.Next(0, 100000);
            Rotdir = Main.rand.NextBool() ? 1 : -1;
            Rotation = Main.rand.NextFloat(0, TwoPi);
        }

        public override void Update()
        {
            // 如果粒子尚未着陆，则执行移动和碰撞检测
            if (!hasLanded)
            {
                // --- 碰撞检测逻辑 ---
                // 将粒子的世界坐标（像素）转换为图格坐标
                Point tileCoordinates = Position.ToTileCoordinates();
                // 安全地获取该位置的图格信息，防止坐标越界导致游戏崩溃
                Tile tile = Framing.GetTileSafely(tileCoordinates.X, tileCoordinates.Y);

                // 检查图格是否为实心且未被虚化（玩家可以穿过虚化的物块）
                if (tile.HasTile && !tile.IsActuated)
                {
                    // 如果是，则粒子“着陆”
                    hasLanded = true;
                    Velocity = Vector2.Zero; // 立即停止移动
                }
                else
                {
                    // --- 原有的移动逻辑 ---
                    // 如果没有着陆，就继续飘落
                    if (Speed != 0)
                    {
                        Vector2 idealVelocity = Vector2.UnitY.RotatedBy(Lerp(-0.94f, 0.94f, (float)Math.Sin(Time / 36f + SeedOffset) * 0.5f + 0.5f)) * Speed;
                        float movementInterpolant = Lerp(0.05f, 0.1f, Utils.GetLerpValue(0, Lifetime / 2, Time, true));
                        Velocity = Vector2.Lerp(Velocity, idealVelocity, movementInterpolant);
                        Velocity = Velocity.SafeNormalize(-Vector2.UnitY) * Speed;
                    }
                }
            }

            // 如果没有着陆，就继续旋转
            if (!hasLanded)
            {
                Rotation += 0.1f * Rotdir;
            }
            Scale = Lerp(BeginScale, 0, EaseInCubic(LifetimeRatio));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = HJScarletTexture.Particle_Leafs.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity * Lighting.Brightness((int)(Position.X / 16f), (int)(Position.Y / 16f)), Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }
}
