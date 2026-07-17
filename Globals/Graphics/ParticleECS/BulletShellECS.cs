using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace HJScarletRework.Globals.Graphics.ParticleECS
{
    public class BulletShellECS : ECSParticleBehavior
    {
        public override void OnSpawn(ref ECSParticleData data)
        {

        }
        public override void Update(ref ECSParticleData data)
        {
            data.aifloat0 += 1;
            data.Rotation += ToRadians(5f) * Math.Sign(data.Velocity.X);
            if (data.aifloat0 >= 5f)
            {
                data.Velocity.Y += 1.15f;
                data.Velocity.X *= 0.96f;
            }
            if (data.aifloat0 >= 20f)
            {
                data.Opacity = Lerp(data.Opacity, 0f, 0.10f);
            }
        }
        public override void Draw(ref ECSParticleData data)
        {
            Texture2D orb = HJScarletTexture.Particle_BulletShell.Value;
            bool fullBright = data.aifloat0 > 0;
            Color bulletColor = fullBright ? data.DrawColor * data.Opacity : data.DrawColor * data.Opacity * Lighting.Brightness((int)(data.Position.X / 16f), (int)(data.Position.Y / 16f));
            Main.spriteBatch.Draw(orb, data.Position - Main.screenPosition, null, bulletColor, data.Rotation, orb.Size() / 2, data.Scale, 0, 0);
        }
    }
}
