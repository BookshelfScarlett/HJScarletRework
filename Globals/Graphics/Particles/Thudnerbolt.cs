using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.ParticleSystem;
using System;
using Terraria;

namespace HJScarletRework.Globals.Graphics.Particles
{
    public class ThunderboltParticle : BaseParticle
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override int UseBlendStateID => BlendStateID.Additive;
        public Vector2 Squish = Vector2.One;
        public float ShakePower;
        public ThunderboltParticle(Vector2 position, float rotation, float scale, Color color, int lifeTime, float shakePower, float opac = 1f, Vector2? squish = null)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            ShakePower = Math.Abs(shakePower);
            DrawColor = color;
            Opacity = opac;
            Lifetime = lifeTime;
            if (squish is not null)
                Squish = squish.Value;
        }
        public override void Update()
        {
            Opacity *= (1- EaseInOutExpo(LifetimeRatio));
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D tex = HJScarletTexture.Particle_ThunderBolt.Value;
            Vector2 shakePos = Vector2.One.RotatedByRandom(TwoPi) * (1 - LifetimeRatio) * ShakePower;
            Vector2 rotationPoint = new Vector2(tex.Width / 2f, tex.Height);
            Color c = Color.Lerp(Color.White, DrawColor, LifetimeRatio);
            SpriteEffects flip = (Main.GlobalTimeWrappedHourly % 30 < 15) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(tex, Position + shakePos - Main.screenPosition, null, DrawColor * Opacity * 0.6f, Rotation, rotationPoint, Squish * Scale, flip, 0);
            spriteBatch.Draw(tex, Position - Main.screenPosition, null, c * Opacity, Rotation, rotationPoint, Squish * Scale, flip, 0);
        }
    }
}
