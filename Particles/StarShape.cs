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
        public bool NoGravity;
        public int BlendStateType;
        public Color SparkColor;
        public override int UseBlendStateID => BlendStateType;
        public StarShape(Vector2 position, Vector2 velocity, Color drawColor, float scale, int lifeTime, int? blendStateID = null, bool noGravity = true)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = drawColor;
            Scale = scale;
            Lifetime = lifeTime;
            BlendStateType = blendStateID ?? BlendStateID.Additive;
            NoGravity = noGravity;
        }
        public override void Update()
        {
            Scale *= 0.95f;
            SparkColor = Color.Lerp(DrawColor, Color.Transparent, (float)Math.Pow(LifetimeRatio, 3D));
            Velocity *= 0.95f;
            if (Velocity.Length() < 12f && !NoGravity)
            {
                Velocity.X *= 0.94f;
                Velocity.Y += 0.25f;
            }
            Rotation = Velocity.ToRotation() + PiOver2;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 scale = new Vector2(0.5f, 1.6f) * Scale;
            Texture2D texture = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, SparkColor, Rotation, texture.Size() * 0.5f, scale, 0, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, SparkColor, Rotation, texture.Size() * 0.5f, scale * new Vector2(0.45f, 1f), 0, 0f);
        }
    }
}
