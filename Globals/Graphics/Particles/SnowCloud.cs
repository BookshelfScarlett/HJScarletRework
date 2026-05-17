using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace HJScarletRework.Globals.Graphics.Particles
{
    public class SnowCloud : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;
        public bool UseAlt;
        public SnowCloud(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale, bool useAlt = false)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
        }
        public override void OnSpawn()
        {
        }

        public override void Update()
        {
            Velocity *= 0.9f;
            Velocity = Velocity.RotatedBy(Main.rand.NextFloat(ToRadians(-5f), ToRadians(5f)));
            Opacity = Lerp(Opacity, Lerp(Opacity, 0, 0.3f), 0.12f);
        }
        // 这里采样没有问题，他贴图就是这样
        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = HJScarletTexture.Texture_SnowCloud.Texture; 
            spriteBatch.Draw(texture.Value, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Value.ToOrigin(), Scale, 0, 0f);
        }
    }
}
