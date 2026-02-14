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
    public class FireShiny : BaseParticle
    {
        public override int UseBlendStateID => BlendStateID.Additive;
        public bool UseRandomTexture = true;
        public int PickLine;
        private Color FireColor;
        private Color TargetColor;
        public FireShiny(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = FireColor = color;
            TargetColor = Color.DarkGray;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
        }
        public FireShiny(Vector2 position, Vector2 velocity, Color beginColor, Color targetColor, int lifetime, float Rot, float opacity, float scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = FireColor = beginColor;
            TargetColor = targetColor;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
        }
        public FireShiny(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale, bool useRandomTexture = true, int pickLine = 0)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = FireColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
            UseRandomTexture = useRandomTexture;
            PickLine = pickLine;
        }

        public override void OnSpawn()
        {
        }

        public override void Update()
        {
            Velocity *= 0.9f;
            Opacity = Lerp(Opacity, Lerp(Opacity, 0, 0.3f), 0.12f);
            //Scale *= 0.93f;
            //火焰的颜色应该逐渐向灰看齐
            //无论是啥火焰都差不多
            DrawColor = Color.Lerp(FireColor, TargetColor, LifetimeRatio);
        }
        // 这里采样没有问题，他贴图就是这样
        public override void Draw(SpriteBatch spriteBatch)
        {
            float brightness = (float)Math.Pow(Lighting.Brightness((int)(Position.X / 16f), (int)(Position.Y / 16f)), 0.15);
            Texture2D texture = HJScarletTexture.Particle_FireShiny.Value;
            if (PickLine > 3)
                PickLine = 0;
            int verticleFrame = UseRandomTexture ? (int)(LifetimeRatio % 4) : PickLine;
            Rectangle frame = HJScarletTexture.Particle_Smoke.Texture.Frame(4, 4, (int)(LifetimeRatio * 16) % 4, (int)(LifetimeRatio * 4));
            Vector2 origin = frame.Size() * 0.5f;
            spriteBatch.Draw(texture, Position - Main.screenPosition, frame, DrawColor * brightness * Opacity, Rotation, origin, Scale, 0, 0f);
        }
    }
}
