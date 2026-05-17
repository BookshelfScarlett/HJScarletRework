using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Rarity.RarityDrawHandler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;

namespace HJScarletRework.Rarity.RarityParticles
{

    public class RaritySnowCloud : RaritySparkle
    {
        public int BlendStateType;
        public override int UseBlendStateID => BlendStateType;
        public bool UseAlt;
        public bool UseAdd;
        public RaritySnowCloud(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale, bool useAlt = false, bool useAdd = false)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Scale = scale;
            Rotation = Rot;
            Opacity = opacity;
            UseAlt = useAlt;
            UseAdd = useAdd;
        }

        public override void CustomUpdate()
        {
            Velocity *= 0.9f;
            Opacity = Lerp(Opacity, Lerp(Opacity, 0, 0.3f), 0.12f);
            Position += Velocity;
        }
        public override void CustomDraw(SpriteBatch spriteBatch, Vector2 drawPosition)
        {
            Asset<Texture2D> texture = HJScarletTexture.Texture_SnowCloud.Texture;
            Vector2 origin = texture.Value.ToOrigin();
            if (UseAdd)
                spriteBatch.Draw(texture.Value, drawPosition, null, DrawColor.ToAddColor() * Opacity, Rotation, origin, Scale, 0, 0f);
            else
                spriteBatch.Draw(texture.Value, drawPosition, null, DrawColor * Opacity, Rotation, origin, Scale, 0, 0f);
        }
    }
}
