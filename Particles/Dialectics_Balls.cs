using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace HJScarletRework.Particles
{
    public class Dialectics_Balls : BaseParticle
    {
        public bool NeedGlowEdge;
        public Color GlowEdgeColor;
        public bool AffectedByGravity;
        public float DrawRotation = 0f;
        public Dialectics_Balls(Vector2 position, Vector2 velocity, int lifetime, Color drawColor, float scale, bool affectedByGravity, bool needGlowEdge = false , Color? glowEdgeColor = null)
        {
            Position = position;
            Velocity = velocity;
            Lifetime = lifetime;
            DrawColor = drawColor;
            Scale = scale;
            NeedGlowEdge = needGlowEdge;
            GlowEdgeColor = glowEdgeColor ?? Color.White;
            AffectedByGravity = affectedByGravity;
        }
        public override void Update()
        {
            Opacity = Lerp(Opacity, Lerp(Opacity, 0, 0.3f), 0.12f);
            Scale *= 0.93f;
            Velocity *= 0.95f;
            if (Velocity.Length() < 12f && AffectedByGravity)
            {
                Velocity.X *= 0.94f;
                Velocity.Y += 0.25f;
            }
            DrawRotation += ToRadians(10f);

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = HJScarletTexture.Specific_DialectBall.Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, DrawColor, DrawRotation, texture.Size() * 0.5f, Scale, 0, 0f);
            //此处，绘制描边
            if (NeedGlowEdge)
            {
                //绘制发光边缘
                for (int i = 0; i < 8; i++)
                    Main.spriteBatch.Draw(texture, Position - Main.screenPosition + ToRadians(i * 60f).ToRotationVector2() * 2f, null, GlowEdgeColor with { A = 0 }, DrawRotation, texture.Size() / 2, Scale, 0, 0f);

            }
        }
    }
}
