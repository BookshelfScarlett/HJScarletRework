using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Graphics.Particles
{
    public class LightningGlow : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;
        private float BeginScale;
        private Color TargetColor;
        private float Offset;
        public LightningGlow(Vector2 position, Vector2 vel, Color color, int lifeTime, float scale)
        {
            Position = position;
            Velocity = vel;
            DrawColor = Color.Transparent;
            TargetColor = color;
            Lifetime = lifeTime;
            Scale = BeginScale= scale;
        }
        public override void Update()
        {
            if (LifetimeRatio < 0.15f)
                DrawColor = Color.Lerp(DrawColor, TargetColor, 0.28f);
            else
                DrawColor = Color.Lerp(DrawColor, Color.Transparent, 0.25f);
            Offset = Main.rand.NextFloat(-2.5f, 2.6f);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 drawPos = Position - Main.screenPosition;
            Texture2D tex = HJScarletTexture.Particle_SharpTear;
            Vector2 drawScale = new Vector2(0.8f, 1.5f);
            for (int i = 0; i < 6; i++)
            {
                Vector2 offsetVec = Velocity.ToSafeNormalize() * i * 1.1f + Velocity.ToSafeNormalize().RotatedBy(PiOver2) * Offset;
                spriteBatch.Draw(tex, drawPos + offsetVec, null, DrawColor, Velocity.ToRotation() + PiOver2, tex.ToOrigin(), Scale * drawScale, 0, 0);
            }
        }
    }
}
