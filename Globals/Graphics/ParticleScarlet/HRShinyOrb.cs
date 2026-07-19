using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleScarlet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HJScarletRework.Globals.Graphics.ParticleScarlet
{
    /// <summary>
    /// 代办：该系统准备废弃
    /// </summary>
    public class HRShinyOrbAlt : ScarletParticle
    {
        public override string Texture => HJScarletTexture.Particle_HRShinyOrb.Path;
        public override BlendState TheBlendState => BlendState.Additive;
        public float GlowCenterMult = 0;
        public override void PostReset()
        {
            GlowCenterMult = 0;
        }
        public override void Update()
        {
            Velocity *= 0.94f;
            Scale = Lerp(Scale, 0, EaseInCubic(LifetimeRatio));
            Velocity = Velocity.RotatedBy(Main.rand.NextFloat(-.03f, 0.03f));
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = HJScarletTexture.Particle_HRShinyOrb.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, 0, texture.Size() / 2, Scale, SpriteEffects.None, 0);
            if (GlowCenterMult > 0)
                spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color.White * Opacity, 0, texture.Size() / 2, Scale * GlowCenterMult, SpriteEffects.None, 0);
        }
    }
}
