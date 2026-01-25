using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using HJScarletRework.Globals.ParticleSystem;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Assets.Registers;

namespace HJScarletRework.Particles
{
    public class CrossGlow : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;
        public float BeginScale;
        public SpriteEffects se = SpriteEffects.None;
        public bool UseFadeIn = true;
        public CrossGlow(Vector2 position, Color color, int lifetime, float opacity, float scale)
        {
            Position = position;
            DrawColor = color;
            Lifetime = lifetime;
            Opacity = opacity;
            Scale = scale;
            BeginScale = scale;
        }
        public CrossGlow(Vector2 position, Color color, int lifetime, float opacity, float scale, bool useFadeIn)
        {
            Position = position;
            DrawColor = color;
            Lifetime = lifetime;
            Opacity = opacity;
            Scale = scale;
            BeginScale = scale;
            UseFadeIn = useFadeIn;
        }
        public override void OnSpawn()
        {
            if (Main.rand.NextBool())
                se = SpriteEffects.FlipHorizontally;

            if (UseFadeIn)
                Scale = BeginScale;
        }
        public override void Update()
        {
            if (LifetimeRatio < 0.5f)
            {
                if (UseFadeIn)
                    Scale = Lerp(0f, BeginScale, EaseOutCubic(LifetimeRatio * 2));
            }
            else
            {
                float progress = LifetimeRatio - 0.5f;
                Scale = Lerp(BeginScale, 0f, EaseOutCubic(progress * 2));
            }
        }
        // 这里采样没有问题，他贴图就是这样
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = HJScarletTexture.Particle_CrossGlow.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, se, 0f);
        }
    }
}
