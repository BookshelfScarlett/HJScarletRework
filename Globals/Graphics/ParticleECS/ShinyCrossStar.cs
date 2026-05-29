using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Globals.Graphics.ParticleECS
{
    public class ShinyCrossStareECS : ECSParticleBehavior
    {
        public override void OnSpawn(ref ECSParticleData particleDate)
        {
        }
        public override void Update(ref ECSParticleData data)
        {
            base.Update(ref data);
            data.Scale *= .93f;
            data.DrawColor *= Lerp(1f, .2f, (float)Math.Pow(data.LifetimeRatio, 30));
            data.Velocity *= .95f;
        }
        public override void Draw(ref ECSParticleData data)
        {
            Texture2D star = HJScarletTexture.Particle_SharpTear;
            Vector2 pos = data.Position - Main.screenPosition;
            float drawMinor = data.aifloat0;
            for (float i = 0; i < 1f; i += drawMinor)
            {
                Vector2 starScale = GetScale(i);
                float colorAlpha = GetAlphaFade(1 - i);
                Main.spriteBatch.Draw(star, pos, null, data.DrawColor * data.Opacity* colorAlpha, 0, star.Size() / 2, starScale * data.Scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(star, pos, null, data.DrawColor * data.Opacity* colorAlpha, 0+ PiOver2, star.Size() / 2, starScale * data.Scale, SpriteEffects.None, 0);
                //Main.spriteBatch.Draw(star, pos, null, Color.White * data.Opacity, 0 + PiOver2, star.Size() / 2, starScale * data.Scale * 0.5f, SpriteEffects.None, 0);
            }

        }
        public float GetAlphaFade(float t)
        {
            return Lerp(0.5f, 1f, t);
        }
        public Vector2 GetScale(float t)
        {
            Vector2 starScale = new(1.2f, 0.8f);
            Vector2 beginScale = new(0.2f, 0.05f);
            return Vector2.Lerp(beginScale, starScale, t);
        }
    }
}
