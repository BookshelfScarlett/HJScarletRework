using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HJScarletRework.Globals.Graphics.Particles
{
    public class PlusParticle : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;
        public float BeginScale;
        public SpriteEffects se = SpriteEffects.None;
        public bool UseBoomLight = false;
        public bool UseFadeIn = true;
        public int BlendstateID;
        private float Accelerating = 0f;
        public bool HasRotation = false;
        public PlusParticle(Vector2 position, Vector2 velocity, Color color, int lifetime, float scale, float accelerating = 1f, bool hasRotation = false, float opacity = 1f)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Opacity = opacity;
            Scale = scale;
            Accelerating = accelerating;
            Rotation = 0;
            HasRotation = hasRotation;
        }
        public override void OnSpawn()
        {
            if (Main.rand.NextBool())
                se = SpriteEffects.FlipHorizontally;

            if (UseFadeIn)
                Scale = BeginScale;
            if (HasRotation)
                Rotation = Velocity.ToRotation();
        }
        public override void Update()
        {
            if (LifetimeRatio > 0.8f)
                Scale = Lerp(Scale, 0f, 0.1f);
            Velocity *= Accelerating;
        }
        // 这里采样没有问题，他贴图就是这样
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = HJScarletTexture.Particle_Plus.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, se, 0f);
        }
    }
}
