using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using HJScarletRework.Globals.ParticleSystem;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Items.Weapons.Melee;

namespace HJScarletRework.Particles
{
    public class WildPointerRed: BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Alpha;
        public WildPointerRed(Vector2 position, int lifetime, float rotation, float scale, float opacity)
        {
            Position = position;
            Lifetime = lifetime;
            Rotation = rotation;
            Scale = scale;
            Opacity = opacity;
            DrawColor = Color.Red;
        }
        public override void OnSpawn()
        {
        }
        public override void Update()
        {
            DrawColor *= (1 - LifetimeRatio); 
        }
        // 这里采样没有问题，他贴图就是这样
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Request<Texture2D>(GetInstance<WildPointerThrown>().Texture).Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, 0, 0f);
        }
    }
}
