using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HJScarletRework.Globals.Graphics.Particles
{
    public class ShinyRing : BaseParticle
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        private bool FadeIn;
        private float TargetScale;
        private float RotSpeed = 0;
        public override int UseBlendStateID => BlendStateID.Additive;
        public ShinyRing(Vector2 position, Vector2 velocity, Color color, int lifeTime, float scale, float rot = 0, float rotSpeed = 0, float opacity = 1f, bool fadeIn = false)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifeTime;
            Rotation = rot;
            Scale = TargetScale = scale;
            FadeIn = fadeIn;
            Opacity = opacity;
            RotSpeed = rotSpeed;
        }
        public override void OnSpawn()
        {
            if (FadeIn)
                Scale = 0;
        }
        public override void Update()
        {
            Velocity *= 0.97f;
            if (Time > (int)(Lifetime * 0.80f))
            {
                Scale = Lerp(Scale, 0f, 0.10f);
                Opacity *= 0.80f;
            }
            else if (FadeIn)
                Scale = Lerp(Scale, TargetScale, 0.3f);
            if (RotSpeed != 0)
                Rotation += RotSpeed;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = HJScarletTexture.Particle_RingShiny.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() * 0.5f, Scale, 0, 0f);
        }
    }
}
