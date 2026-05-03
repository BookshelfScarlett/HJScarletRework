using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.ParticleSystem;
using HJScarletRework.Rarity.RarityDrawHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace HJScarletRework.Rarity.RarityParticles
{
    public class RarityCrossStar : RaritySparkle
    {
        public bool UseRot = false;
        public override int UseBlendStateID => BlendStateID.Additive;
        public Color InitColor;
        public float SpinSpeed = 0;
        private float BeginScale;
        public RarityCrossStar(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale, float spinSpeed = 0f)
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
        public override void CustomUpdate()
        {
            Scale *= 0.974f;
            DrawColor = Color.Lerp(InitColor, InitColor * 0.2f, (float)Math.Pow(LifetimeRatio, 30));
            Velocity *= 0.95f;
            Rotation += SpinSpeed;
            //太小的情况下直接处死粒子就行了
            if (Scale < BeginScale * 0.15f)
                Time = Lifetime;
        }
        public override void CustomDraw(SpriteBatch spriteBatch, Vector2 drawPosition)
        {
            Texture2D star = HJScarletTexture.Particle_HRStar.Value;
            Tex2DWithPath shinyOrb = HJScarletTexture.Particle_ShinyOrb;
            Vector2 drawPos = drawPosition;

            spriteBatch.Draw(star, drawPos, null, DrawColor * Opacity, Rotation, star.Size() / 2, Scale, SpriteEffects.None, 0);
        }
    }
}
