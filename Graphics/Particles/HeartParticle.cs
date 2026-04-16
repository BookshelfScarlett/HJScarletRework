using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HJScarletRework.Graphics.Particles
{
    public class HeartParticle : BaseParticle
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        private int SetBlendState;
        private int HeartType;
        private bool FadeIn;
        private float TargetScale;
        public override int UseBlendStateID => SetBlendState;
        public HeartParticle(Vector2 position, Vector2 velocity, Color color, int lifeTime, float scale, float opacity, int? blendState = null, int heartType = 0, bool fadeIn = false)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifeTime;
            Scale = TargetScale = scale;
            SetBlendState = blendState ?? BlendStateID.Additive;
            HeartType = heartType;
            Opacity = opacity;
            FadeIn = fadeIn;
        }
        public override void OnSpawn()
        {
            if (FadeIn)
                Scale = 0;
        }
        public override void Update()
        {
            Velocity *= 0.97f;
            if (Time > (int)(Lifetime * 0.75f))
            {
                Scale = Lerp(Scale, 0f, 0.15f);
                Opacity *= 0.99f;
                Velocity *= .91f;
            }
            else if (FadeIn)
                Scale = Lerp(Scale, TargetScale, 0.2f);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = HeartType switch
            {
                1 => HJScarletTexture.Particle_HeartHalfFill.Value,
                2 => HJScarletTexture.Particle_HeartNoFill.Value,
                _ => HJScarletTexture.Particle_HeartFullFill.Value,
            };
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, 0, texture.Size() * 0.5f, Scale, 0, 0f);
            base.Draw(spriteBatch);
        }
    }
}
