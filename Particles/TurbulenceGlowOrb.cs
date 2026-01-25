using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Particles
{
    public class TurbulenceGlowOrb : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;
        public float Speed = 5f;
        public int SeedOffset = 0;
        public float BeginScale = 1f;
        public float TurBulenceDirection = 0f;
        public bool DrawGlowCenter;
        public float GlowCenterMult;
        public TurbulenceGlowOrb(Vector2 position, float speed, Color color, int lifetime, float scale, float direction, bool drawGlowCenter = false, float glowCenterMult = 0.5f)
        {
            Position = position;
            Speed = speed;
            DrawColor = color;
            Lifetime = lifetime;
            Scale = scale;
            BeginScale = scale;
            TurBulenceDirection = direction;
            DrawGlowCenter = drawGlowCenter;
            GlowCenterMult = glowCenterMult;
        }
        public override void OnSpawn()
        {
            SeedOffset = Main.rand.Next(0, 100000);
        }

        public override void Update()
        {
            if (Speed != 0)
            {
                Vector2 idealVelocity = -Vector2.UnitY.RotatedBy(Lerp(-TurBulenceDirection, TurBulenceDirection, (float)Math.Sin(Time / 36f + SeedOffset) * 0.5f + 0.5f)) * Speed;
                float movementInterpolant = Lerp(0.01f, 0.25f, Utils.GetLerpValue(0, Lifetime / 2, Time, true));
                Velocity = Vector2.Lerp(Velocity, idealVelocity, movementInterpolant);
                Velocity = Velocity.SafeNormalize(-Vector2.UnitY) * Speed;
            }
            Velocity *= 0.9f;
            Scale = Lerp(BeginScale, 0, EaseOutCubic(LifetimeRatio));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = HJScarletTexture.Particle_HRShinyOrbSmall.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, texture.Size() / 2, Scale, SpriteEffects.None, 0);
            if (DrawGlowCenter)
                spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color.White * Opacity, Rotation, texture.Size() / 2, Scale * GlowCenterMult, SpriteEffects.None, 0);

        }
    }
}
