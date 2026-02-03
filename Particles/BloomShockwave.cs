using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HJScarletRework.Particles
{

    public class BloomShockwave : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;
        public float BeginScale;
        public bool FadeIn;
        public BloomShockwave(Vector2 position, Color color, int lifetime, float opacity, float scale, bool fadeIn = true)
        {
            Position = position;
            DrawColor = color;
            Lifetime = lifetime;
            Opacity = opacity;
            Scale = scale;
            BeginScale = scale;
            FadeIn = fadeIn;
        }
        public override void OnSpawn()
        {
        }
        public override void Update()
        {
            if (LifetimeRatio < 0.5f && FadeIn)
            {
                Scale = Lerp(BeginScale/2f, BeginScale, EaseOutCubic(LifetimeRatio * 2));
            }
            else
            {
                float progress = LifetimeRatio - 0.5f;
                Opacity = Lerp(1f, 0f, EaseOutCubic(progress * 2));
            }
        }
        // 这里采样没有问题，他贴图就是这样
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = HJScarletTexture.Texture_BloomShockwave.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, 0, 0f);
        }
    }
}
