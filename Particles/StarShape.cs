using Microsoft.Xna.Framework;
using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using HJScarletRework.Core.ParticleSystem;

namespace HJScarletRework.Particles
{
    public class StarShape : BaseParticle
    {
        public int BlendStateType;
        public bool NoGravity = true;
        public Color SparkColor;
        public bool DrawGlow = true;
        public float GlowScale = 0.45f;
        public bool HasRotation;
        public float BeginScale = 0;
        public override int UseBlendStateID => BlendStateType;
        public StarShape(Vector2 position, Vector2 velocity, Color drawColor, float scale, int lifeTime)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = drawColor;
            Scale = BeginScale =scale;
            Lifetime = lifeTime;
            BlendStateType = BlendStateID.Additive;
        }
        public StarShape(Vector2 position, Vector2 velocity, Color drawColor, float scale, int lifeTime, bool drawGlow)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = drawColor;
            Scale = BeginScale = scale;
            Lifetime = lifeTime;
            BlendStateType = BlendStateID.Additive;
            HasRotation = false;
            DrawGlow = drawGlow;
        }
        public StarShape(Vector2 position, Vector2 velocity, Color drawColor, float scale, int lifeTime, float rot)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = drawColor;
            Scale = BeginScale =scale;
            Lifetime = lifeTime;
            BlendStateType = BlendStateID.Additive;
            HasRotation = true;
            Rotation = rot;
        }
        public override void Update()
        {
            if (!HasRotation)
                Scale *= 0.95f;
            else
                Scale *= 0.99f;
            SparkColor = Color.Lerp(DrawColor, Color.Transparent, (float)Math.Pow(LifetimeRatio, 3D));
            Velocity *= 0.95f;
            if (Velocity.Length() < 12f && !NoGravity)
            {
                Velocity.X *= 0.94f;
                Velocity.Y += 0.25f;
            }
            Rotation = HasRotation ? Rotation : Velocity.ToRotation() + PiOver2;
            if (Scale < 0.05f * BeginScale)
                Time = Lifetime;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 scale = new Vector2(0.5f, 1.6f) * Scale;
            Texture2D texture = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, SparkColor, Rotation, texture.Size() * 0.5f, scale, 0, 0f);
            if (DrawGlow)
                spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color.White, Rotation, texture.Size() * 0.5f, scale * 0.5f, 0, 0f);
        }
    }
}
