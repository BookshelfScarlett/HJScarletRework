using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;

namespace HJScarletRework.Globals.Graphics.ParticleECS
{
    public class LiliesFire : ECSParticleBehavior
    {

        public override void Update(ref ECSParticleData particleDate)
        {
            particleDate.Velocity *= .9f;
            particleDate.Opacity = Lerp(particleDate.Opacity, 0, 0.12f);
        }
        public override void Draw(ref ECSParticleData data)
        {
            bool fullBright = data.aifloat0 > 0;
            float brightness = (float)Math.Pow(Lighting.Brightness((int)(data.Position.X / 16f), (int)(data.Position.Y / 16f)), 0.15);
            Color drawColor = data.DrawColor * brightness * data.Opacity;
            if (fullBright)
            {
                drawColor = data.DrawColor * data.Opacity;
            }
            Asset<Texture2D> texture = HJScarletTexture.Particle_Fire.Texture;

            Rectangle frame = texture.Frame(8, 8, (int)(data.LifetimeRatio * 64) % 8, (int)(data.LifetimeRatio * 8));
            Vector2 origin = frame.Size() * 0.5f;
            Main.spriteBatch.Draw(texture.Value, data.Position - Main.screenPosition, frame, drawColor, data.Rotation, origin, data.Scale, SpriteEffects.None, 0);
        }
    }
}
