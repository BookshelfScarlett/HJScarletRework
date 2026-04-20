using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
                DrawColor = Color.Lerp(DrawColor, TargetColor, 0.35f);
            else
                DrawColor = Color.Lerp(DrawColor, Color.Transparent, 0.25f);
            Offset = Main.rand.NextFloat(-1.1f, 1.2f);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 drawPos = Position - Main.screenPosition;
            Texture2D tex = HJScarletTexture.Particle_SharpTear;
            //Vector2 drawScale = new Vector2(1.0f, 1.8f);
            Vector2 drawScale = new Vector2(0.5f, 3.5f);
            //for (int i = 0; i < 6; i++)
            //{
            //    Vector2 offsetVec = Velocity.ToSafeNormalize() * i * 1.4f + Velocity.ToSafeNormalize().RotatedBy(PiOver2) * Offset;
            //    spriteBatch.Draw(tex, drawPos + offsetVec, null, DrawColor, Velocity.ToRotation() + PiOver2, tex.ToOrigin(), Scale * drawScale, 0, 0);
            //}
            for (int i = 0; i < 6; i++)
            {
                Vector2 offsetVec = Velocity.ToSafeNormalize() * i * 1.4f;
                spriteBatch.Draw(tex, drawPos + offsetVec, null, DrawColor, Velocity.ToRotation() + PiOver2, tex.ToOrigin(), Scale * drawScale, 0, 0);
            }
        }
    }
}
