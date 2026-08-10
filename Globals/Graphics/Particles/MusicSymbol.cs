using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.ParticleSystem;
using System;
using Terraria;

namespace HJScarletRework.Globals.Graphics.Particles
{
    public class MusicSymbol : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Alpha;
        public float RandSeedValue = 0;
        public MusicSymbol(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
        }
        public override void OnSpawn()
        {
            RandSeedValue = Main.rand.NextFloat(MathHelper.ToRadians(-25f), MathHelper.ToRadians(25f));
            Opacity = 0f;
        }

        public override void Update()
        {
            if (LifetimeRatio > 0.85f)
            {
                Opacity = Lerp(Opacity, 0, 0.21f);
            }
            else
                Opacity = Lerp(Opacity, 1, 0.1f);
        }
        // 这里采样没有问题，他贴图就是这样
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = HJScarletTexture.Particle_MusicSymbol.Value;
            float rotOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5) * .45f;
            //最终旋转角度
            float finalRotation = Rotation * RandSeedValue + rotOffset;
            Color edgeC = Color.Lerp(Color.Transparent, Color.White, Opacity);
            //for(int i =0; i <8;i++)
            //spriteBatch.Draw(texture, Position + (TwoPi/8f*i).ToRotationVector2()*2*Opacity - Main.screenPosition, null, edgeC.ToAddColor(), finalRotation, Vector2.Zero, Scale, 0, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, finalRotation, Vector2.Zero, Scale, 0, 0f);
        }
    }
}
