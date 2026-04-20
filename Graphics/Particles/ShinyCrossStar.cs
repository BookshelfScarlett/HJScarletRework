using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace HJScarletRework.Graphics.Particles
{
    public class ShinyCrossStar : BaseParticle
    {
        public bool UseRot = false;
        public override int UseBlendStateID => BlendStateID.Additive;
        public Color InitColor;
        public float SpinSpeed = 0;
        private float BeginScale;
        public bool UseLegacy = true;
        public ShinyCrossStar(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale, float spinSpeed = 0f)
        {
            Position = position;
            Velocity = velocity;
            InitColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = BeginScale = scale;
            SpinSpeed = spinSpeed;
        }
        public ShinyCrossStar(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale, bool useLegacy, float spinSpeed = 0f)
        {
            Position = position;
            Velocity = velocity;
            InitColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = BeginScale = scale;
            SpinSpeed = spinSpeed;
            UseLegacy = useLegacy;
        }
        public override void OnSpawn()
        {
        }

        public override void Update()
        {
            Scale *= 0.93f;
            DrawColor = Color.Lerp(InitColor, InitColor * 0.2f, (float)Math.Pow(LifetimeRatio, 30));
            Velocity *= 0.95f;
            Rotation += SpinSpeed;
            //太小的情况下直接处死粒子就行了
            if (Scale < BeginScale * 0.1f)
                Time = Lifetime;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D star = HJScarletTexture.Particle_SharpTear;
            Tex2DWithPath shinyOrb = HJScarletTexture.Particle_ShinyOrb;
            Vector2 drawPos = Position - Main.screenPosition;
            if(UseLegacy)
            {
                Vector2 starScale = new(1.2f, 0.8f);
                spriteBatch.Draw(star, drawPos, null, DrawColor * Opacity, Rotation, star.Size() / 2, starScale * Scale, SpriteEffects.None, 0);
                spriteBatch.Draw(star, drawPos, null, DrawColor * Opacity, Rotation + PiOver2, star.Size() / 2, starScale * Scale, SpriteEffects.None, 0);
                spriteBatch.Draw(shinyOrb.Value, drawPos, null, Color.Lerp(Color.White, DrawColor, 0.5f) * 0.95f * Opacity, 0, shinyOrb.Origin, Scale * 0.75f, SpriteEffects.None, 0);
            }
            else
                for (float i = 0; i < 1f; i += 0.1f)
                {
                    Vector2 starScale = GetScale(i);
                    float colorAlpha = GetAlphaFade(1-i);
                    spriteBatch.Draw(star, drawPos, null, DrawColor * Opacity*colorAlpha, Rotation, star.Size() / 2, starScale * Scale, SpriteEffects.None, 0);
                    spriteBatch.Draw(star, drawPos, null, DrawColor * Opacity*colorAlpha, Rotation + PiOver2, star.Size() / 2, starScale * Scale, SpriteEffects.None, 0);
                    spriteBatch.Draw(star, drawPos, null, Color.White * Opacity, Rotation + PiOver2, star.Size() / 2, starScale * Scale * 0.5f, SpriteEffects.None, 0);
                }
            //防止过曝
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
