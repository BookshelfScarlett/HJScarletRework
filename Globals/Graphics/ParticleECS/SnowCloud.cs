using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace HJScarletRework.Globals.Graphics.ParticleECS
{
    public class SnowCloudECS : ECSParticleBehavior
    {

        public override void Update(ref ECSParticleData particleDate)
        {
            particleDate.Velocity *= .9f;
            particleDate.Velocity.RotatedBy(Main.rand.NextFloat(ToRadians(-5f), ToRadians(5f)));
            particleDate.Opacity = Lerp(particleDate.Opacity, Lerp(particleDate.Opacity, 0, .3f), 0.12f);
        }
        public override void Draw(ref ECSParticleData data)
        {
            Asset<Texture2D> texture = HJScarletTexture.Texture_SnowCloud.Texture;
            Main.spriteBatch.Draw(texture.Value, data.Position - Main.screenPosition, null, data.DrawColor * data.Opacity, data.Rotation, texture.Size() / 2, data.Scale, SpriteEffects.None, 0);

        }
    }
}
