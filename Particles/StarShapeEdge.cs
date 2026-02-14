using Microsoft.Xna.Framework;
using HJScarletRework.Globals.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using HJScarletRework.Core.ParticleSystem;
using System.Runtime.Intrinsics.Arm;
using HJScarletRework.Globals.Methods;

namespace HJScarletRework.Particles
{
    public class StarShapeEdge : BaseParticle
    {
        public int BlendStateType;
        private Color SparkColor;
        private Color EdgeColor;
        private float BeginScale;
        public bool DrawGlow = true;
        public float GlowScale = 0.45f;
        public override int UseBlendStateID => BlendStateType;
        public StarShapeEdge(Vector2 position, Vector2 velocity, Color drawColor, Color edgeColor,float scale, float rot, int lifeTime)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = drawColor;
            EdgeColor = edgeColor;
            Scale = BeginScale = scale;
            Rotation = rot; 
            Lifetime = lifeTime;
            BlendStateType = BlendStateID.Additive;
        }
       public override void Update()
        {
            Scale *= 0.95f;
            SparkColor = Color.Lerp(DrawColor, Color.Transparent, (float)Math.Pow(LifetimeRatio, 3D));
            Velocity *= 0.95f;
            //Scale大于0.45的时候把TimeLeft的值直接设置为……Time的大小，以处死射弹
            if (Scale <= BeginScale * 0.20f)
                Time = Lifetime;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 scale = new Vector2(0.5f, 1.6f) * Scale;
            Vector2 edgeScale = new Vector2(0.5f, 1.6f) * Scale;
            Texture2D texture = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Vector2 drawPos = Position - Main.screenPosition;
            for (int i = 0; i< 8;i++)
                spriteBatch.Draw(texture, drawPos + ToRadians(60 * i).ToRotationVector2() * 2f, null, EdgeColor, Rotation, texture.Size() / 2, edgeScale, 0, 0f);
            spriteBatch.Draw(texture, drawPos, null, SparkColor, Rotation, texture.Size() / 2, scale, 0, 0f);
        }
    }
}
